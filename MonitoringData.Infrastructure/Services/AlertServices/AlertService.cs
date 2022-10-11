using System.Globalization;
using Microsoft.AspNetCore.SignalR;
using MonitoringSystem.Shared.Data;
using MonitoringData.Infrastructure.Services.DataAccess;
using MonitoringData.Infrastructure.Services.AlertServices;
using Microsoft.Extensions.Logging;
using MonitoringData.Infrastructure.Data;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.SignalR;

namespace MonitoringData.Infrastructure.Services {

    public interface IAlertService {
        Task ProcessAlerts(IList<AlertRecord> items,DateTime now);
        Task DeviceOfflineAlert();
        Task Load();
        Task Reload();
    }

    public class AlertService : IAlertService {
        private readonly IAlertRepo _alertRepo;
        private readonly ILogger<AlertService> _logger;
        private readonly IEmailService _emailService;
        private readonly List<AlertRecord> _activeAlerts = new List<AlertRecord>();
        private readonly IHubContext<MonitorHub, IMonitorHub> _monitorHub;


        public AlertService(IAlertRepo alertRepo,ILogger<AlertService> logger,
            IHubContext<MonitorHub,IMonitorHub> monitorHub,
            IEmailService emailService) {
            this._alertRepo = alertRepo;
            this._logger = logger;
            this._emailService = emailService;
            this._monitorHub = monitorHub;
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
                analogData = alerts.Where(e => e.Enabled && e.ItemType == AlertItemType.Analog)
                    .Select(e => new ItemStatus() {
                        Item = e.DisplayName,
                        State = e.CurrentState.ToString(),
                        Value = e.ChannelReading.ToString(CultureInfo.InvariantCulture)
                    }).ToList(),
                discreteData = alerts.Where(e => e.Enabled && e.ItemType == AlertItemType.Discrete)
                    .Select(e => new ItemStatus() {
                        Item = e.DisplayName,
                        State = e.CurrentState.ToString(),
                        Value = e.ChannelReading.ToString(CultureInfo.InvariantCulture)
                    }).ToList(),
                virtualData = alerts.Where(e => e.Enabled && e.ItemType == AlertItemType.Virtual)
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
                        alert.ChannelReading.ToString(CultureInfo.InvariantCulture));
                }
                if (sendEmail) {
                    await this._emailService.SendMessageAsync(this._alertRepo.ManagedDevice.DeviceName+" Alerts", 
                        messageBuilder);
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
        
        public async Task Load() {
            await this._alertRepo.Load();
        }
        
        public async Task Reload() {
            await this._alertRepo.Reload();
            this._activeAlerts.Clear();
        }
    }
}
