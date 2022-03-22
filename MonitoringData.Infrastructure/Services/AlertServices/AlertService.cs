using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleTables;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Services {
    public enum AlertAction {
        Clear,
        ChangeState,
        Start,
        Resend,
        ShowStatus,
        RemainActive
    }

    public interface IAlertService {
        Task ProcessAlerts(IList<ItemAlert> items);
        bool IsActive(int alertId);
    }

    public class AlertService : IAlertService {
        private readonly IMonitorDataService _dataService;
        private List<ItemAlert> _activeAlerts = new List<ItemAlert>();

        public AlertService(IMonitorDataService dataService) {
            this._dataService = dataService;
        }

        public Task ProcessAlerts(IList<ItemAlert> items) {
            //List<ItemAlert> status = new List<ItemAlert>();
            //List<ItemAlert> newAlerts = new List<ItemAlert>();
            ConsoleTable statusTable = new ConsoleTable("Alert","Status","Reading");
            ConsoleTable newAlertTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable newStateTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable activeTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable resendTable = new ConsoleTable("Alert", "Status", "Reading");
            StringBuilder statusAlerts = new StringBuilder();
            StringBuilder newAlerts = new StringBuilder();
            StringBuilder newStates = new StringBuilder();
            StringBuilder activeAlerts = new StringBuilder();
            StringBuilder resendAlerts = new StringBuilder();
            foreach (var item in this.Process(items)) {
                switch (item.AlertAction) {
                    case AlertAction.Clear: {
                            this._activeAlerts.Remove(item.ActiveAlert);
                            break;
                        }
                    case AlertAction.ChangeState: {
                            this._activeAlerts.Remove(item.ActiveAlert);
                            this._activeAlerts.Add(item);
                            newStateTable.AddRow(item.Alert.displayName, item.Alert.CurrentState.ToString(), item.Reading.ToString());
                            break;
                        }
                    case AlertAction.Start: {
                            this._activeAlerts.Add(item);
                            break;
                        }
                    case AlertAction.Resend: {
                            this._activeAlerts.Remove(item.ActiveAlert);
                            this._activeAlerts.Add(item);
                            resendTable.AddRow(item.Alert.displayName, item.Alert.CurrentState.ToString(), item.Reading.ToString());                         
                            break;
                        }
                    case AlertAction.ShowStatus: {
                            break;
                        }
                    case AlertAction.RemainActive: {
                            break;
                        }
                }
                statusTable.AddRow(item.Alert.displayName, item.Alert.CurrentState.ToString(), item.Reading.ToString());
            }
            foreach(var active in this._activeAlerts) {
                activeTable.AddRow(active.Alert.displayName, active.Alert.CurrentState.ToString(), active.Reading.ToString());
            }

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
            Console.WriteLine();

            return Task.CompletedTask;
        }

        public bool IsActive(int alertId) {
            return (this._activeAlerts.FirstOrDefault(e => e.Alert._id == alertId) != null);
        }

        private IEnumerable<ItemAlert> Process(IList<ItemAlert> itemAlerts) {
            foreach(var itemAlert in itemAlerts) {
                var alertId = itemAlert.Alert._id;
                var actionType = itemAlert.AlertReading.value;
                var activeAlert = this._activeAlerts.FirstOrDefault(e => e.Alert._id == alertId);
                var actionItem = this._dataService.ActionItems.FirstOrDefault(e => e.actionType == actionType);
                var now = DateTime.Now;
                switch (itemAlert.AlertReading.value) {
                    case ActionType.Okay: {
                            if (activeAlert != null) {
                                itemAlert.AlertAction = AlertAction.Clear;
                                itemAlert.Alert.latched = false;
                                itemAlert.ActiveAlert = activeAlert;
                                break;
                            } else {
                                itemAlert.AlertAction = AlertAction.ShowStatus;
                                itemAlert.ActiveAlert = null;
                                break;
                            }
                        }
                    case ActionType.Alarm:
                    case ActionType.Warning:
                    case ActionType.SoftWarn: {
                            if (activeAlert != null) {
                                if (activeAlert.Alert.CurrentState != itemAlert.AlertReading.value) {
                                    itemAlert.Alert.lastAlarm = now;
                                    itemAlert.Alert.latched = true;
                                    itemAlert.Alert.CurrentState = itemAlert.AlertReading.value;
                                    itemAlert.AlertAction = AlertAction.ChangeState;
                                    itemAlert.ActiveAlert = activeAlert;
                                } else {
                                    if ((now - activeAlert.Alert.lastAlarm).TotalSeconds >= 5) {
                                        itemAlert.AlertAction = AlertAction.Resend;
                                        itemAlert.Alert.lastAlarm = now;
                                    } else {
                                        itemAlert.AlertAction = AlertAction.RemainActive;
                                    }
                                }
                                itemAlert.ActiveAlert = activeAlert;
                            } else {
                                itemAlert.AlertAction = AlertAction.Start;
                                itemAlert.Alert.lastAlarm = now;
                                itemAlert.Alert.CurrentState = itemAlert.AlertReading.value;
                                itemAlert.ActiveAlert = null;
                            }
                            break;
                        }
                    case ActionType.Maintenance:
                    case ActionType.Custom:
                    default:
                        itemAlert.AlertAction = AlertAction.ShowStatus;
                        itemAlert.ActiveAlert = null;
                        break;
                }
                yield return itemAlert;
            }
        }
    }
}
