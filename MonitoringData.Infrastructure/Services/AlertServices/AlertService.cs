using System.Globalization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MonitoringData.Infrastructure.Data;
using MonitoringData.Infrastructure.Services.DataAccess;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.SignalR;

namespace MonitoringData.Infrastructure.Services.AlertServices {

    public interface IAlertService {
        Task ProcessAlerts(IList<AlertRecord> items,DateTime now);
        Task DeviceOfflineAlert();
        Task Load();
        Task Reload();
    }
    //Testing github actions infrastructure 9
    public class AlertService : IAlertService {
        private readonly IAlertRepo _alertRepo;
        private readonly ILogger<AlertService> _logger;
        private readonly IEmailService _emailService;
        private readonly ExchangeEmailService _externalEmailService;
        private readonly List<AlertRecord> _activeAlerts = new List<AlertRecord>();
        private readonly IHubContext<MonitorHub, IMonitorHub> _monitorHub;

        public AlertService(IAlertRepo alertRepo,ILogger<AlertService> logger,
            IHubContext<MonitorHub,IMonitorHub> monitorHub,
            IEmailService emailService,ExchangeEmailService externalEmailService) {
            this._alertRepo = alertRepo;
            this._logger = logger;
            this._emailService = emailService;
            this._monitorHub = monitorHub;
            this._externalEmailService = externalEmailService;
        }

        public async Task ProcessAlerts(IList<AlertRecord> alerts,DateTime now) {
            IMessageBuilder messageBuilder = new MessageBuilder();
            messageBuilder.StartMessage(this._alertRepo.ManagedDevice.DeviceName ?? "DeviceNameNotFound");
            bool sendEmail = false;     
            foreach (var alert in alerts) {
                if (alert.Enabled) {
                    var activeAlert = this._activeAlerts.FirstOrDefault(e => e.AlertId == alert.AlertId);
                    var actionItem = this._alertRepo.ActionItems.FirstOrDefault(e => e.ActionType == alert.CurrentState);
                    switch (alert.CurrentState) {
                        case ActionType.Okay: {
                                if (activeAlert != null) {
                                    if (!activeAlert.Latched) {
                                        activeAlert.Latched = true;
                                        activeAlert.TimeLatched = now;
                                    } else {
                                        if ((now - activeAlert.TimeLatched).TotalSeconds >= 60) {
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
                                            var emailPeriod = actionItem.EmailPeriod<=0 ? 30 : actionItem.EmailPeriod;
                                            if ((now - activeAlert.LastAlert).TotalMinutes >= emailPeriod) {
                                                activeAlert.LastAlert = now;
                                                activeAlert.ChannelReading = alert.ChannelReading;
                                                sendEmail = true;
                                            } else {
                                                activeAlert.ChannelReading = alert.ChannelReading;
                                            }
                                        } else {
                                            this._logger.LogError("ActiveAlert not found in Alarm/Warning/SoftWarn");
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
                    messageBuilder.AppendStatus(alert.DisplayName, alert.CurrentState.ToString(), 
                        alert.ChannelReading.ToString(CultureInfo.InvariantCulture));
                }
            }
            MonitorData monitorData = new MonitorData {
                TimeStamp = now,
                analogData = alerts.Where(e => e.Display && e.ItemType == AlertItemType.Analog)
                    .Select(e => new ItemStatus() {
                        Item = e.DisplayName,
                        State = e.CurrentState.ToString(),
                        Value = e.ChannelReading.ToString("N0")
                    }).ToList(),
                discreteData = alerts.Where(e => e.Display && e.ItemType == AlertItemType.Discrete)
                    .Select(e => new ItemStatus() {
                        Item = e.DisplayName,
                        State = e.CurrentState.ToString(),
                        Value = e.ChannelReading.ToString(CultureInfo.InvariantCulture)
                    }).ToList(),
                virtualData = alerts.Where(e => e.Display && e.ItemType == AlertItemType.Virtual)
                    .Select(e => new ItemStatus() {
                        Item = e.DisplayName,
                        State = e.CurrentState.ToString(),
                        Value = e.ChannelReading.ToString(CultureInfo.InvariantCulture)
                    }).ToList()
            };
            if (this._activeAlerts.Any()) {
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
                        Value = alert.ChannelReading.ToString(CultureInfo.InvariantCulture)
                    });
                    messageBuilder.AppendAlert(alert.DisplayName, alert.CurrentState.ToString(), 
                        alert.ChannelReading.ToString("N1"));
                }
                if (sendEmail) {
                    await this._emailService.SendMessageAsync(this._alertRepo.ManagedDevice.DeviceName+" Alerts", 
                        messageBuilder);
                    var bulkH2 = this._activeAlerts.FirstOrDefault(e => e.DisplayName == "Bulk H2(PSI)");
                    var bulkN2 = this._activeAlerts.FirstOrDefault(e => e.DisplayName == "Bulk N2(inH20)");
                    await this.ProcessBulkGasExternalEmail(bulkN2, bulkH2,now);
                    this._logger.LogInformation("Email Sent");
                    var alertReadings = alerts.Select(e => new AlertReading() {
                        MonitorItemId = e.AlertId,
                        AlertState = e.CurrentState,
                        Reading = e.ChannelReading
                    });
                    await this._alertRepo.LogAlerts(new AlertReadings() {
                        readings = alertReadings.ToArray(), timestamp = now
                    });
                }
            } else {
                monitorData.DeviceState = alerts.FirstOrDefault(e => e.CurrentState == ActionType.Maintenance)!=null ? 
                    ActionType.Maintenance : ActionType.Okay;
            }
            await this._monitorHub.Clients.All.ShowCurrent(monitorData);
        }
        public async Task DeviceOfflineAlert() {
            IMessageBuilder messageBuilder = new MessageBuilder();
            messageBuilder.StartMessage(this._alertRepo.ManagedDevice.DeviceName);
            messageBuilder.AppendAlert(this._alertRepo.ManagedDevice.DeviceName, "Offline", "Offline");
            MonitorData monitorData = new MonitorData {
                TimeStamp = DateTime.Now,
                DeviceState = ActionType.Alarm
            };
            monitorData.activeAlerts = new List<ItemStatus>() {
                new ItemStatus() {
                    Item=this._alertRepo.ManagedDevice.DeviceName,
                    State = "Offline",
                    Value="Offline"
                }
            };
            await this._emailService.SendMessageAsync(this._alertRepo.ManagedDevice.DeviceName+" Alerts", 
                messageBuilder);
            await this._monitorHub.Clients.All.ShowCurrent(monitorData);
        }
        
        private bool CheckBypassReset(AlertRecord alert,DateTime now) {
            return (now - alert.TimeBypassed).Minutes >= alert.BypassResetTime;
        }

        private async Task ProcessBulkGasExternalEmail(AlertRecord? bulkN2,AlertRecord? bulkH2,DateTime now) {
            if (bulkN2 != null) {
                if (bulkN2.LastAlert == now) {
                    switch (bulkN2.CurrentState) {
                        case ActionType.Warning: {
                            await this._externalEmailService.SendMessageAsync("Nitrogen EMERGENCY Gas Refill Request", 
                                "Nitrogen",
                                bulkN2.ChannelReading.ToString(CultureInfo.InvariantCulture),"inH2O", 
                                "Immediately");
                            break;
                        }
                        case ActionType.SoftWarn: {
                            await this._externalEmailService.SendMessageAsync("Nitrogen Gas Refill Request", 
                                "Nitrogen",
                                bulkN2.ChannelReading.ToString(CultureInfo.InvariantCulture),"inH2O", 
                                "within the next 24 Hrs");
                            break;
                        }
                        default: break;//do nothing
                    }
                }
            }
            if (bulkH2 != null) {
                if (bulkH2.LastAlert == now) {
                    switch (bulkH2.CurrentState) {
                        case ActionType.Warning: {
                            await this._externalEmailService.SendMessageAsync("Hydrogen Gas EMERGENCY Refill Request", 
                                "Hydrogen",
                                bulkH2.ChannelReading.ToString(CultureInfo.InvariantCulture),"PSI", 
                                "Immediately");
                            break;
                        }
                        case ActionType.SoftWarn: {
                            await this._externalEmailService.SendMessageAsync("Hydrogen Gas Refill Request", 
                                "Hydrogen",
                                bulkH2.ChannelReading.ToString(CultureInfo.InvariantCulture),"PSI", 
                                "within the next 24 Hrs");
                            break;
                        }
                        default: break;//do nothing
                    }
                }
            }
        }
        
        public async Task Load() {
            await this._alertRepo.Load();
        }
        
        public async Task Reload() {
            await this._alertRepo.Reload();
            this._activeAlerts.Clear();
        }
    }
}
