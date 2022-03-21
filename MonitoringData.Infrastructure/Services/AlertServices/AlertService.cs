using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Services {
    public interface IAlertService {
        AlertAction CheckAlert(AlertReading reading);
        void ProcessAlerts(IList<ItemAlert> items);
        bool IsActive(int alertId);
        Task Start();
    }

    public class AlertService : IAlertService {
        private readonly IMonitorDataService _dataService;
        private List<MonitorAlert> _activeAlerts = new List<MonitorAlert>();

        public AlertService(IMonitorDataService dataService) {
            this._dataService = dataService;
        }

        public bool IsActive(int alertId) {
            return (this._activeAlerts.FirstOrDefault(e => e._id == alertId) != null);
        }

        public AlertAction CheckAlert(AlertReading reading) {
            var activeAlert = this._activeAlerts.FirstOrDefault(e => e._id == reading.itemid);
            var actionItem = this._dataService.ActionItems.FirstOrDefault(e => e.actionType == reading.value);
            var now = DateTime.Now;
            switch (reading.value) {
                case ActionType.Okay: {
                        if (activeAlert != null) {
                            return AlertAction.Clear;
                        } else {
                            return AlertAction.Nothing;
                        }
                    }
                case ActionType.Alarm:
                case ActionType.Warning:
                case ActionType.SoftWarn: {
                        if (activeAlert != null) {
                            if (activeAlert.CurrentState != reading.value) {

                            }
                            if ((now - activeAlert.lastAlarm).TotalMinutes >= actionItem.EmailPeriod) {
                                return AlertAction.Resend;
                            } else {
                                return AlertAction.Nothing;
                            }
                        } else {
                            return AlertAction.Start;
                        }
                    }
                case ActionType.Maintenance:
                case ActionType.Custom:
                    return AlertAction.Nothing;
                default:
                    return AlertAction.Nothing;
            }
        }

        public void RecieveAlert() {

        }


    }
}
