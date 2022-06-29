using ConsoleTables;
using Microsoft.AspNetCore.SignalR;
using MonitoringSystem.Shared.Data;
using MonitoringData.Infrastructure.Services.DataAccess;
using MonitoringData.Infrastructure.Services.AlertServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MonitoringSystem.Shared.SignalR;

namespace MonitoringData.Infrastructure.Services {

    public interface IAlertService {
        Task ProcessAlerts(IList<AlertRecord> items,DateTime now);
        Task Initialize();
    }

    public class AlertService : IAlertService {
        private readonly IAlertRepo _alertRepo;
        private readonly ILogger<AlertService> _logger;
        private readonly MonitorDatabaseSettings _settings;
        private readonly IEmailService _emailService;
        private List<AlertRecord> _activeAlerts = new List<AlertRecord>();
        private readonly IHubContext<MonitorHub, IMonitorHub> _monitorHub;


        public AlertService(IAlertRepo alertRepo,ILogger<AlertService> logger,
            IOptions<MonitorDatabaseSettings> options,
            IHubContext<MonitorHub,IMonitorHub> monitorHub,
            IEmailService emailService) {
            this._alertRepo = alertRepo;
            this._logger = logger;
            this._emailService = emailService;
            this._monitorHub = monitorHub;
            this._settings = options.Value;
        }

        public AlertService(string connName,string databaseName,string actionCol, string alertCol) {
            this._alertRepo = new AlertRepo(connName, databaseName, actionCol, alertCol,"alert_readings");
            this._emailService = new ExchangeEmailService();
        }

        public async Task ProcessAlerts(IList<AlertRecord> alerts,DateTime now) {
            IMessageBuilder messageBuilder = new MessageBuilder();
            messageBuilder.StartMessage(this._settings.EmailSubject);
            ConsoleTable statusTable = new ConsoleTable("Alert","Status","Reading");
            ConsoleTable newAlertTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable newStateTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable activeTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable resendTable = new ConsoleTable("Alert", "Status", "Reading");
            bool sendEmail = false;     
            foreach (var alert in this.Process(alerts)) {
                if (alert.Enabled) {
                    switch (alert.AlertAction) {
                        case AlertAction.Clear: {
                                this._activeAlerts.RemoveAll(e=>e.AlertId==alert.AlertId);
                                break;
                            }
                        case AlertAction.ChangeState: {
                                var activeAlert = this._activeAlerts.FirstOrDefault(e => e.AlertId == alert.AlertId);
                                if (activeAlert != null) {
                                    activeAlert.CurrentState = alert.CurrentState;
                                    activeAlert.ChannelReading = alert.ChannelReading;
                                    activeAlert.AlertAction = alert.AlertAction;
                                } else {
                                    //Log Error
                                }
                                newStateTable.AddRow(alert.DisplayName, alert.CurrentState.ToString(), alert.ChannelReading.ToString());
                                messageBuilder.AppendChanged(alert.DisplayName, alert.CurrentState.ToString(), alert.ChannelReading.ToString());
                                sendEmail = true;
                                break;
                            }
                        case AlertAction.Start: {
                                this._activeAlerts.Add(alert.Clone());
                                sendEmail = true;
                                break;
                            }
                        case AlertAction.Resend: {
                                resendTable.AddRow(alert.DisplayName, alert.CurrentState.ToString(), alert.ChannelReading.ToString());
                                sendEmail = true;
                                break;
                            }
                    }
                    statusTable.AddRow(alert.DisplayName, alert.CurrentState.ToString(), alert.ChannelReading.ToString());
                    messageBuilder.AppendStatus(alert.DisplayName, alert.CurrentState.ToString(), alert.ChannelReading.ToString());
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
                    messageBuilder.AppendAlert(alert.DisplayName, alert.CurrentState.ToString(), alert.ChannelReading.ToString());
                }
                if (sendEmail) {
                    await this._emailService.SendMessageAsync(this._settings.EmailSubject+" Alerts", 
                        messageBuilder.FinishMessage());
                    var alertReadings = alerts.Select(e => new AlertReading() {
                        itemid = e.AlertId,
                        reading = e.ChannelReading,
                        state = e.CurrentState
                    });
                    await this._alertRepo.LogAlerts(new AlertReadings() { readings = alertReadings.ToArray(), timestamp = now });
                }
            } else {
                monitorData.DeviceState = alerts.FirstOrDefault(e => e.CurrentState == ActionType.Maintenance)!=null ? 
                    ActionType.Maintenance : ActionType.Okay;
            }
            await this._monitorHub.Clients.All.ShowCurrent(monitorData);
            Console.Clear();
            Console.WriteLine("New Alerts:");
            Console.WriteLine(newAlertTable.ToMinimalString());
            Console.WriteLine();
            Console.WriteLine("Active Alerts");
            Console.WriteLine(activeTable.ToMinimalString());
            Console.WriteLine();
            Console.WriteLine("Resend Alerts");
            Console.WriteLine(resendTable.ToMinimalString());
            Console.WriteLine();
            Console.WriteLine("ChangeState Alerts");
            Console.WriteLine(newStateTable.ToMinimalString());
            Console.WriteLine();
            Console.WriteLine("Status");
            Console.WriteLine(statusTable.ToMinimalString());
            Console.WriteLine();
        }
        private IEnumerable<AlertRecord> Process(IList<AlertRecord> alertRecords) {
            var now = DateTime.Now;
            foreach (var alert in alertRecords) {
                var activeAlert = this._activeAlerts.FirstOrDefault(e => e.AlertId == alert.AlertId);
                var actionItem = this._alertRepo.ActionItems.FirstOrDefault(e => e.actionType == alert.CurrentState);
                switch (alert.CurrentState) {
                    case ActionType.Okay: {
                            if (activeAlert != null) {
                                if ((now - activeAlert.LastAlert).TotalSeconds >= 60) {
                                    alert.AlertAction = AlertAction.Clear;
                                } else {
                                    alert.AlertAction = AlertAction.Nothing;
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
                                        if ((now - activeAlert.LastAlert).TotalMinutes >= actionItem.EmailPeriod) {
                                            alert.AlertAction = AlertAction.Resend;
                                            alert.LastAlert = now;
                                        } else {
                                            alert.AlertAction = AlertAction.Nothing;
                                        }
                                    } else {
                                        //log error-ActionItem not found
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
            await this._alertRepo.Load();
        }
    }
}
