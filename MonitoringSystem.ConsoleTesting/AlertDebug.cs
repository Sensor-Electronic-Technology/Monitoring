using System;
using MonitoringSystem.Shared.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConsoleTables;
using MonitoringData.Infrastructure.Services.DataAccess;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Data;
using MonitoringData.Infrastructure.Services;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringSystem.Shared.Services;
using MonitoringSystem.Shared.SignalR;

namespace MonitoringSystem.ConsoleTesting {
    public class AlertDebugMain {
        static readonly CancellationTokenSource s_cts = new CancellationTokenSource();
        static async Task Main(string[] args) {
            AlertProcessing alertService = new AlertProcessing();
            ActionType actionType = ActionType.Okay;
            bool firstRun = true;
            DateTime last=DateTime.Now;
            while (true) {
                var alert = new AlertRecord() {
                    AlertId = 1,
                    CurrentState = actionType,
                    DisplayName = "Alert 1",
                    Enabled = true,
                    ChannelReading = 1
                };
                if (firstRun) {
                    firstRun = false;
                    alert.CurrentState = actionType;
                } else {
                    if ((DateTime.Now - last).TotalSeconds >= 5) {
                        actionType = actionType == ActionType.Okay ? ActionType.Alarm : ActionType.Okay;
                        alert.CurrentState = actionType;
                        last=DateTime.Now;
                    }
                }
                List<AlertRecord> alerts = new List<AlertRecord>() {
                    new AlertRecord() {
                        AlertId = 2,CurrentState = ActionType.Okay, DisplayName = "Alert 2", Enabled = true, ChannelReading = 1
                    },
                    new AlertRecord() {
                        AlertId = 3,CurrentState = ActionType.Okay, DisplayName = "Alert 3", Enabled = true, ChannelReading = 1
                    },
                    new AlertRecord() {
                        AlertId = 4,CurrentState = ActionType.Okay, DisplayName = "Alert 4", Enabled = true, ChannelReading = 1
                    }
                };
                alerts.Add(alert);
                await alertService.ProcessAlerts(alerts,DateTime.Now);
                await Task.Delay(1000);
            }
        }

        static async Task DebugAlertsFull() {
            var client = new MongoClient("mongodb://172.20.3.41");
            var settings = new MonitorDataLogSettings();
            settings.ConnectionString = "mongodb://172.20.3.41";
            settings.DatabaseName = "monitor_settings";
            settings.EmailRecipientCollection = "email_recipients";
            settings.ManagedDeviceCollection = "monitor_devices";
            var emailSettings = new MonitorEmailSettings();
            emailSettings.SmtpHost = "192.168.0.123";
            emailSettings.SmtpPort = 25;
            emailSettings.FromUser = "Alert Troubleshooting";
            emailSettings.FromAddress = "monitoralerts@s-et.com";
            var configProvider = new DataLogConfigProvider(new MongoClient("mongodb://172.20.3.41"), settings, emailSettings);
            configProvider.DeviceName = "gasbay";
            await configProvider.Load();
            var alertService = new AlertProcessing(new MongoClient("mongodb://172.20.3.41"), configProvider);
            var dataService = new MonitorDataService(new MongoClient("mongodb://172.20.3.41"), configProvider,null);
            var dataLogger = new DataLogger(dataService, alertService);
            await dataLogger.Load();
            bool first = true;
            while (true) {
                await dataLogger.Run(first);
                first = false;
            }
        }
    }

    public class DataLogger {
        private MonitorDataService _dataService;
        private DataLogConfigProvider _provider;
        private AlertProcessing _alertService;
        
        public DataLogger(MonitorDataService dataService,AlertProcessing alertService) {
            this._dataService = dataService;
            this._alertService = alertService;
        }

        public async Task Run(bool first) {
            List<AlertRecord> alerts = new List<AlertRecord>();
           
            DateTime triggerTime=DateTime.Now;
            
            foreach (var alert in this._dataService.MonitorAlerts) {
                DateTime now=DateTime.Now;
                if (alert._id == 125) {
                    /*var state = alert.CurrentState != ActionType.Alarm ? ActionType.Alarm : ActionType.Okay;
                    alert.CurrentState = state;*/
                    if (first) {
                        alerts.Add(new AlertRecord(alert,ActionType.Alarm));
                    } else {
                        alerts.Add(new AlertRecord(alert,ActionType.Okay));
                        /*if ((now - triggerTime).TotalSeconds >= 10) {
                            alerts.Add(new AlertRecord(alert,ActionType.Okay));
                        } else {
                            alerts.Add(new AlertRecord(alert,ActionType.Alarm));
                        }*/
                    }
                } else {
                    alerts.Add(new AlertRecord(alert,ActionType.Okay));
                }
            }
            await this._alertService.ProcessAlerts(alerts,DateTime.Now);
            await Task.Delay(1000);
        }

        public async Task Load() {
            await this._dataService.LoadAsync();
            await this._alertService.Initialize();
        }
    }

    public class AlertProcessing {
        /*private readonly IAlertRepo _alertRepo;

        private readonly IEmailService _emailService;*/
        private List<AlertRecord> _activeAlerts = new List<AlertRecord>();
        private string EmailSubject;
        private int emailCount = 0;
        
        public AlertProcessing(IMongoClient client,DataLogConfigProvider provider) {
            //this._alertRepo = new AlertRepo(client,provider);
            //this._emailService = new SmtpEmailService(provider);
        }

        public AlertProcessing() {
            
        }
        
        public async Task ProcessAlerts(IList<AlertRecord> alerts, DateTime now) {
            //IMessageBuilder messageBuilder = new MessageBuilder();
            //messageBuilder.StartMessage(this._alertRepo.ManagedDevice.DeviceName);
            ConsoleTable statusTable = new ConsoleTable("Alert","Status","Reading");
            ConsoleTable newAlertTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable newStateTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable activeTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable resendTable = new ConsoleTable("Alert", "Status", "Reading");
            bool sendEmail = false;
            foreach (var alert in alerts) {
                var activeAlert = this._activeAlerts.FirstOrDefault(e => e.AlertId == alert.AlertId);
                //var actionItem = this._alertRepo.ActionItems.FirstOrDefault(e => e.actionType == alert.CurrentState);
                var actionItem = new ActionItem() {
                    actionType = ActionType.Alarm,
                    EmailEnabled = true,
                    EmailPeriod = 20
                };
                switch (alert.CurrentState) {
                    case ActionType.Okay: {
                            if (activeAlert != null) {
                                if (!activeAlert.Latched) {
                                    activeAlert.Latched = true;
                                    activeAlert.TimeLatched = now;
                                } else {
                                    if ((now - activeAlert.TimeLatched).TotalSeconds >= 3) {
                                        this._activeAlerts.Remove(activeAlert);
                                    }
                                }
                            }//else do nothing, there is no activeAlert
                            break;
                        }
                    case ActionType.Alarm:
                    case ActionType.Warning:
                    case ActionType.SoftWarn: {
                            if (activeAlert != null) {
                                if (activeAlert.CurrentState != alert.CurrentState) {
                                    activeAlert.CurrentState = alert.CurrentState;
                                    activeAlert.ChannelReading = alert.ChannelReading;
                                    activeAlert.AlertAction = alert.AlertAction;
                                    activeAlert.LastAlert = now;
                                    sendEmail = true;
                                } else {
                                    if (activeAlert.Latched) {
                                        activeAlert.Latched = false;
                                        activeAlert.TimeLatched = now;
                                    }
                                    if (actionItem != null) {
                                        var emailPeriod = actionItem.EmailPeriod<=0  ? 20 : actionItem.EmailPeriod;
                                        if ((now - activeAlert.LastAlert).TotalSeconds >= emailPeriod) {
                                            activeAlert.LastAlert = now;
                                            activeAlert.ChannelReading = alert.ChannelReading;
                                            sendEmail = true;
                                        } else {
                                            activeAlert.ChannelReading = alert.ChannelReading;
                                        }
                                    } else {
                                        //this._logger.LogError("ActiveAlert not found in Alarm/Warning/SoftWarn");
                                        //could not find actionItem,should never be here
                                    }
                                }
                            } else {
                                alert.LastAlert = now;
                                this._activeAlerts.Add(alert.Clone());
                                sendEmail = true;
                            }
                            break;
                        }
                    case ActionType.Maintenance:
                    case ActionType.Custom:
                    default:
                        //do nothing on maintenance,custom
                        break;
                }
            }
            
            MonitorData monitorData = new MonitorData();
            monitorData.TimeStamp = now;
            
            monitorData.analogData = alerts.Where(e => e.Enabled && e.ItemType == AlertItemType.Analog)
                .Select(e => new ItemStatus() {
                    Item = e.DisplayName,
                    State = e.CurrentState.ToString(),
                    Value = e.ChannelReading.ToString()
                }).ToList();

            monitorData.discreteData = alerts.Where(e => e.Enabled && e.ItemType == AlertItemType.Discrete)
                .Select(e => new ItemStatus() {
                    Item = e.DisplayName,
                    State = e.CurrentState.ToString(),
                    Value = e.ChannelReading.ToString()
                }).ToList();

            monitorData.virtualData = alerts.Where(e => e.Enabled && e.ItemType == AlertItemType.Virtual)
                .Select(e => new ItemStatus() {
                    Item = e.DisplayName,
                    State = e.CurrentState.ToString(),
                    Value = e.ChannelReading.ToString()
                }).ToList();
            
            foreach (var alert in alerts) {
                statusTable.AddRow(alert.DisplayName, alert.CurrentState.ToString(), alert.ChannelReading.ToString());
            }
            if (this._activeAlerts.Count > 0) {
                monitorData.activeAlerts = new List<ItemStatus>();
                if (this._activeAlerts.FirstOrDefault(e => e.CurrentState == ActionType.Alarm)!=null) {
                    monitorData.DeviceState = ActionType.Alarm;
                }else if (this._activeAlerts.FirstOrDefault(e => e.CurrentState == ActionType.Warning) != null) {
                    monitorData.DeviceState = ActionType.Warning;
                } else if (this._activeAlerts.FirstOrDefault(e => e.CurrentState == ActionType.SoftWarn) != null) {
                    monitorData.DeviceState = ActionType.SoftWarn;
                }
                foreach (var alert in this._activeAlerts) {
                    monitorData.activeAlerts.Add(new ItemStatus() {
                        Item=alert.DisplayName,
                        State=alert.CurrentState.ToString(),
                        Value = alert.ChannelReading.ToString()
                    });
                    activeTable.AddRow(alert.DisplayName, alert.CurrentState.ToString(), alert.ChannelReading.ToString());
                    //messageBuilder.AppendAlert(alert.DisplayName, alert.CurrentState.ToString(), alert.ChannelReading.ToString());
                }

                if (sendEmail) {
                    this.emailCount++;
                    var alertReadings = alerts.Select(e => new AlertReading() {
                        itemid = e.AlertId,
                        reading = e.ChannelReading,
                        state = e.CurrentState
                    });
                }
            } else {
                monitorData.DeviceState = alerts.FirstOrDefault(e => e.CurrentState == ActionType.Maintenance)!=null ? 
                    ActionType.Maintenance : ActionType.Okay;
            }
            //await this._monitorHub.Clients.All.ShowCurrent(monitorData);
            
            /*Console.WriteLine("New Alerts:");
            Console.WriteLine(newAlertTable.ToMinimalString());
            Console.WriteLine();*/
            Console.Clear();
            if (sendEmail) {
                Console.WriteLine("Email Sent");
            }
            Console.WriteLine($"Email Count: {this.emailCount}");
            Console.WriteLine("Active Alerts");
            Console.WriteLine(activeTable.ToMinimalString());
            Console.WriteLine();
            Console.WriteLine("Status");
            Console.WriteLine(statusTable.ToMinimalString());
            Console.WriteLine();
            /*Console.WriteLine("Resend Alerts");
            Console.WriteLine(resendTable.ToMinimalString());
            Console.WriteLine();*/
            /*Console.WriteLine("ChangeState Alerts");
            Console.WriteLine(newStateTable.ToMinimalString());
            Console.WriteLine();
            Console.WriteLine("Status");
            Console.WriteLine(statusTable.ToMinimalString());
            Console.WriteLine();*/
        }
        private IEnumerable<AlertRecord> Process(IList<AlertRecord> alertRecords) {
            var now = DateTime.Now;
            foreach (var alert in alertRecords) {
                var activeAlert = this._activeAlerts.FirstOrDefault(e => e.AlertId == alert.AlertId);
                //var actionItem = this._alertRepo.ActionItems.FirstOrDefault(e => e.actionType == alert.CurrentState);
                ActionItem actionItem = new ActionItem() {
                    actionType = ActionType.Alarm, EmailEnabled = true, display = true, EmailPeriod = 10
                };
                switch (alert.CurrentState) {
                    case ActionType.Okay: {
                            if (activeAlert != null) {
                                if (!activeAlert.Latched) {
                                    activeAlert.Latched = true;
                                    activeAlert.TimeLatched = now;
                                    alert.AlertAction = AlertAction.Nothing;
                                } else {
                                    alert.AlertAction = ((now - activeAlert.TimeLatched).TotalSeconds >= 10)
                                        ? AlertAction.Clear : AlertAction.Nothing;
                                }
                            } else {
                                alert.AlertAction = AlertAction.Nothing;
                            }
                            break;
                        }
                    case ActionType.Alarm:
                    case ActionType.Warning:
                    case ActionType.SoftWarn: {
                            if (activeAlert != null) {
                                if (activeAlert.CurrentState != alert.CurrentState) {
                                    alert.LastAlert = now;
                                    alert.AlertAction = AlertAction.ChangeState;
                                } else {
                                    if (actionItem != null) {
                                        if (activeAlert.Latched) {
                                            activeAlert.Latched = false;
                                            activeAlert.TimeLatched = now;
                                        }
                                        if ((now - activeAlert.LastAlert).TotalSeconds >= 5) {
                                            activeAlert.LastAlert = now;
                                            alert.AlertAction = AlertAction.Resend;
                                            alert.LastAlert = now;
                                        } else {
                                            alert.AlertAction = AlertAction.Nothing;
                                        }
                                    } else {
                                        Console.WriteLine("Error: ActiveAlert not found in Alarm/Warning/SoftWarn");
                                        alert.AlertAction = AlertAction.Nothing;
                                    }
                                }
                            } else {
                                alert.AlertAction = AlertAction.Start;
                                alert.LastAlert = now;
                            }
                            break;
                        }
                    case ActionType.Maintenance:
                    case ActionType.Custom:
                    default:
                        alert.AlertAction = AlertAction.Nothing;
                        break;
                }
                yield return alert;
            }
        }
                
        private bool CheckBypassReset(AlertRecord alert,DateTime now) {
            return (now - alert.TimeBypassed).Minutes >= alert.BypassResetTime;
        }
        
        public async Task Initialize() {
            //await this._alertRepo.Load();
        }
    }
} 



