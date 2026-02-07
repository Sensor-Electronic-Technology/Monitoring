using System.Globalization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
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
    
    public class AlertService : IAlertService {
        private readonly IAlertRepo _alertRepo;
        private readonly ILogger<AlertService> _logger;
        private readonly IEmailService _emailService;
        private readonly ExchangeEmailService _externalEmailService;
        private readonly List<AlertRecord> _activeAlerts = new List<AlertRecord>();
        private readonly IHubContext<MonitorHub, IMonitorHub> _monitorHub;
        private DateTime _tankWarmupStart;

        public AlertService(IAlertRepo alertRepo,ILogger<AlertService> logger,
            IHubContext<MonitorHub,IMonitorHub> monitorHub,
            IEmailService emailService,ExchangeEmailService externalEmailService) {
            this._alertRepo = alertRepo;
            this._logger = logger;
            this._emailService = emailService;
            this._monitorHub = monitorHub;
            this._externalEmailService = externalEmailService;
            this._tankWarmupStart=DateTime.Now;
        }

        public async Task ProcessAlerts(IList<AlertRecord> alerts,DateTime now) {
            IMessageBuilder messageBuilder = new MessageBuilder();
            messageBuilder.StartMessage(this._alertRepo.ManagedDevice.DeviceName ?? "DeviceNameNotFound");
            bool sendEmail = false;
            bool sendExEmail = false;
            await this.CheckBypassAlert(now);
            foreach (var alert in alerts) {
                if (alert.Enabled) {
                    var activeAlert = this._activeAlerts.FirstOrDefault(e => e.AlertId == alert.AlertId);
                    var actionItem = this._alertRepo.ActionItems.FirstOrDefault(e => e.ActionType == alert.CurrentState);
                    var bypassAlert=this._alertRepo.BypassAlerts.FirstOrDefault(e=>e._id==alert.AlertId);
                    switch (alert.CurrentState) {
                        case ActionType.Okay: {
                                if (activeAlert != null) {
                                    if (activeAlert.AlertLatched) {
                                        activeAlert.AlertLatched = false;
                                        activeAlert.TimeAlertLatched = now;
                                    }
                                    if (!activeAlert.Latched) {
                                        activeAlert.Latched = true;
                                        activeAlert.TimeLatched = now;
                                    } else {
                                        if (activeAlert.DisplayName is "Bulk H2(PSI)" or "Bulk N2(inH20)" or "Silane" or "Tank1 Weight" or "Tank2 Weight") {
                                            if ((now - activeAlert.TimeLatched).TotalMinutes >= 5) {
                                                this._activeAlerts.Remove(activeAlert);
                                            }
                                        } else {
                                            if ((now - activeAlert.TimeLatched).TotalSeconds >= 60) {
                                                this._activeAlerts.Remove(activeAlert);
                                            }
                                        }
                                    }
                                }//else do nothing, there is no activeAlert
                                break;
                            }
                        case ActionType.Alarm:
                        case ActionType.Warning:
                        case ActionType.SoftWarn: {
                            if (activeAlert == null) {
                                var newAlert = alert.Clone();
                                newAlert.AlertLatched = true;
                                newAlert.TimeAlertLatched = now;
                                newAlert.CurrentState = ActionType.Okay;
                                this._activeAlerts.Add(newAlert);
                                this._logger.LogInformation("Alert: {Alert} Latched",newAlert.DisplayName);
                            } else {
                                activeAlert.Bypassed = bypassAlert?.Bypassed ?? false;
                                activeAlert.TimeBypassed = bypassAlert?.TimeBypassed ?? DateTime.MaxValue ;
                                activeAlert.BypassResetTime = bypassAlert?.BypassResetTime ?? 24;
                                
                                if (activeAlert.CurrentState != alert.CurrentState) {
                                    if (activeAlert.Latched) {
                                        activeAlert.Latched = false;
                                        activeAlert.TimeLatched = now;
                                    }
                                    if (!activeAlert.AlertLatched) {
                                        activeAlert.AlertLatched = true;
                                        activeAlert.TimeAlertLatched = now;
                                        this._logger.LogInformation("Alert: {Alert} Latched",activeAlert.DisplayName);
                                    } else {
                                        if (activeAlert.DisplayName is "Tank1 Weight" or "Tank2 Weight") {
                                            if ((now - activeAlert.TimeAlertLatched).TotalMinutes >= 5) {
                                                activeAlert.ChannelReading = alert.ChannelReading;
                                                activeAlert.AlertAction = alert.AlertAction;
                                                activeAlert.LastAlert = now;
                                                if (activeAlert.CurrentState == ActionType.Okay) {
                                                    sendEmail = true;
                                                } else {
                                                    sendEmail = activeAlert.CurrentState < alert.CurrentState;
                                                }
                                                activeAlert.CurrentState = alert.CurrentState;
                                                this._logger.LogInformation("NH3 Alert({Alert}) Email Check.  5minutes",activeAlert.DisplayName);
                                            }
                                        }else {
                                            if ((now - activeAlert.TimeAlertLatched).TotalSeconds >= 30) {
                                                activeAlert.ChannelReading = alert.ChannelReading;
                                                activeAlert.AlertAction = alert.AlertAction;
                                                activeAlert.LastAlert = now;
                                                if (activeAlert.DisplayName is  "Bulk N2(inH20)" or "Silane" or "Bulk H2(PSI)") {
                                                    if (activeAlert.DisplayName is "Silane") {
                                                        if (activeAlert.CurrentState == ActionType.Okay) {
                                                            if (!activeAlert.Bypassed) {
                                                                sendEmail = true;
                                                            }
                                                        } else {
                                                            if (!activeAlert.Bypassed) {
                                                                sendEmail = activeAlert.CurrentState < alert.CurrentState;
                                                            }
                                                        }
                                                    } else if(activeAlert.DisplayName is "Bulk H2(PSI)") {
                                                        if (activeAlert.CurrentState == ActionType.Okay) {
                                                            sendExEmail = true;
                                                        } else {
                                                            sendExEmail = activeAlert.CurrentState < alert.CurrentState;
                                                        }

                                                        if (alert.CurrentState == ActionType.Alarm) {
                                                            if (!activeAlert.Bypassed) {
                                                                sendEmail = true;
                                                            }
                                                        }
                                                    }else {
                                                        if (activeAlert.CurrentState == ActionType.Okay) {
                                                            if (!activeAlert.Bypassed) {
                                                                sendEmail = true;
                                                            }
                                                            sendExEmail = sendEmail;
                                                        } else {
                                                            if (!activeAlert.Bypassed) {
                                                                sendEmail = activeAlert.CurrentState < alert.CurrentState;
                                                            }
                                                            sendExEmail = sendEmail;
                                                        }
                                                    }
                                                }else {
                                                    if (!activeAlert.Bypassed) {
                                                        sendEmail = true;
                                                    }
                                                }
                                                activeAlert.CurrentState = alert.CurrentState;
                                                this._logger.LogInformation("Alert({Alert}) Email Check. 30seconds",activeAlert.DisplayName);
                                            }
                                        }
                                    }
                                } else {
                                    //Current state has not changed - determine if repeat interval has passed and send email if needed
                                    if (activeAlert.AlertLatched) {
                                        activeAlert.AlertLatched = false;
                                        activeAlert.TimeAlertLatched = now;
                                        this._logger.LogInformation("Alert: {Alert} un-latched",activeAlert.DisplayName);
                                    }
                                    if (activeAlert.Latched) {
                                        activeAlert.Latched = false;
                                        activeAlert.TimeLatched = now;
                                    }
                                    if (actionItem != null) {
                                        if (activeAlert.DisplayName is "Bulk H2(PSI)" or "Bulk N2(inH20)" or "Silane" or "Tank1 Weight" or "Tank2 Weight") {
                                            //Is BulkGas - Do not repeat emails at set interval
                                            activeAlert.ChannelReading = alert.ChannelReading;
                                        } else {
                                            //Is not BulkGas - Repeat emails at set interval
                                            var emailPeriod = actionItem.EmailPeriod<=0 ? 30 : actionItem.EmailPeriod;
                                            if ((now - activeAlert.LastAlert).TotalMinutes >= emailPeriod) {
                                                activeAlert.LastAlert = now;
                                                activeAlert.ChannelReading = alert.ChannelReading;
                                                if (!activeAlert.Bypassed) {
                                                    sendEmail = true;
                                                }
                                                this._logger.LogInformation("Resend Alert: {Alert}",activeAlert.DisplayName);
                                            } else {
                                                activeAlert.ChannelReading = alert.ChannelReading;
                                            }
                                        }
                                    } else {
                                        this._logger.LogError("ActiveAlert not found in Alarm/Warning/SoftWarn");
                                    }
                                }
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
                } else if (this._activeAlerts.FirstOrDefault(e => e.CurrentState == ActionType.SoftWarn || e.CurrentState==ActionType.Okay) != null) {
                    monitorData.DeviceState = ActionType.SoftWarn;
                }
                foreach (var alert in this._activeAlerts) {
                    monitorData.activeAlerts.Add(new ItemStatus() {
                        Item = alert.DisplayName,
                        State = (alert.CurrentState==ActionType.Okay) ? ActionType.SoftWarn.ToString() : alert.CurrentState.ToString(),
                        Value = alert.ChannelReading.ToString(CultureInfo.InvariantCulture)
                    });
                    messageBuilder.AppendAlert(alert.DisplayName, alert.CurrentState.ToString(), 
                        alert.ChannelReading.ToString("N1"));
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
                
                if (sendExEmail) {
                    var bulkH2 = this._activeAlerts.FirstOrDefault(e => e.DisplayName == "Bulk H2(PSI)");
                    var bulkN2 = this._activeAlerts.FirstOrDefault(e => e.DisplayName == "Bulk N2(inH20)");
                    await this.ProcessBulkGasExternalEmail(bulkN2, bulkH2,now);
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
            await this._emailService.SendMessageAsync(this._alertRepo.ManagedDevice.DeviceName+" Alerts", messageBuilder);
            await this._monitorHub.Clients.All.ShowCurrent(monitorData);
        }

        private async Task CheckBypassAlert(DateTime now) {
            var bypassed = this._activeAlerts.Where(e => e.Bypassed).ToList();
            if (bypassed.Count>0) {
                List<UpdateOneModel<BypassAlert>> updates=new List<UpdateOneModel<BypassAlert>>();
                foreach (var alert in bypassed) {
                    if ((now - alert.TimeBypassed).TotalMinutes >= alert.BypassResetTime) {
                        var filter = Builders<BypassAlert>.Filter.Eq(e => e._id,alert.AlertId);
                        var update = Builders<BypassAlert>.Update
                            .Set(e => e.Bypassed, false)
                            .Set(e => e.TimeBypassed,DateTime.MaxValue);
                        updates.Add(new UpdateOneModel<BypassAlert>(
                            filter,update));
                    }
                }

                if (updates.Count > 0) {
                    await this._alertRepo.ResetBypassAlerts(updates);
                }
            }
        }
        
        private bool CheckBypassReset(AlertRecord alert,DateTime now) {
            return (now - alert.TimeBypassed).Hours >= alert.BypassResetTime;
        }

        private async Task ProcessBulkGasExternalEmail(AlertRecord? bulkN2,AlertRecord? bulkH2,DateTime now) {
            if (bulkN2 != null) {
                if (bulkN2.LastAlert == now) {
                    switch (bulkN2.CurrentState) {
                        case ActionType.Alarm: {
                            await this._externalEmailService.SendN2MessageAsync("Nitrogen EMERGENCY Gas Refill Request", 
                                "Nitrogen",
                                bulkN2.ChannelReading.ToString(CultureInfo.InvariantCulture),"inH2O", 
                                "Immediately");
                            break;
                        }
                        case ActionType.Warning: {
                            await this._externalEmailService.SendN2MessageAsync("Nitrogen EMERGENCY Gas Refill Request", 
                                "Nitrogen",
                                bulkN2.ChannelReading.ToString(CultureInfo.InvariantCulture),"inH2O", 
                                "within the next 8 Hrs");
                            break;
                        }
                        case ActionType.SoftWarn: {
                            await this._externalEmailService.SendN2MessageAsync("Nitrogen Gas Refill Request", 
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
                        case ActionType.Alarm: {
                            await this._externalEmailService.SendH2MessageAsync("Hydrogen Gas EMERGENCY Refill Request", 
                                "Hydrogen",
                                bulkH2.ChannelReading.ToString(CultureInfo.InvariantCulture),"PSI", 
                                "Immediately");
                            break;
                        }
                        case ActionType.Warning: {
                            await this._externalEmailService.SendH2MessageAsync("Hydrogen Gas EMERGENCY Refill Request", 
                                "Hydrogen",
                                bulkH2.ChannelReading.ToString(CultureInfo.InvariantCulture),"PSI", 
                                "within the next 12Hrs");
                            break;
                        }
                        case ActionType.SoftWarn: {
                            await this._externalEmailService.SendH2MessageAsync("Hydrogen Gas Refill Request", 
                                "Hydrogen",
                                bulkH2.ChannelReading.ToString(CultureInfo.InvariantCulture),"PSI", 
                                "within the next 48 Hrs");
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
