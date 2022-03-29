using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleTables;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services.DataAccess;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringSystem.Shared.Data;
using Microsoft.Extensions.Logging;
using MassTransit;
using MonitoringSystem.Shared.Contracts;

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
        Task Initialize();
    }

    public class AlertService : IAlertService {
        private readonly IAlertRepo _alertRepo;
        private readonly ILogger<AlertService> _logger;
        private readonly ISendEndpointProvider _sendEnpoint;
        private IEmailService _emailService;
        private List<ItemAlert> _activeAlerts = new List<ItemAlert>();
        private readonly object _locker = new object();

        public AlertService(IAlertRepo alertRepo,ILogger<AlertService> logger) {
            this._alertRepo = alertRepo;
            this._logger = logger;
            this._emailService = new EmailService();
            //this._sendEnpoint = sendEndpoint;
        }

        public AlertService(string connName,string databaseName,string actionCol, string alertCol) {
            this._alertRepo = new AlertRepo(connName, databaseName, actionCol, alertCol);
            this._emailService = new EmailService();
        }

        public async Task ProcessAlerts(IList<ItemAlert> items) {
            IMessageBuilder messageBuilder = new MessageBuilder();
            messageBuilder.StartMessage();
            ConsoleTable statusTable = new ConsoleTable("Alert","Status","Reading");
            ConsoleTable newAlertTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable newStateTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable activeTable = new ConsoleTable("Alert", "Status", "Reading");
            ConsoleTable resendTable = new ConsoleTable("Alert", "Status", "Reading");
            bool sendEmail = false;
            
            foreach (var item in this.Process(items)) {
                if (item.Alert.enabled) {
                    switch (item.AlertAction) {
                        case AlertAction.Clear: {
                                this._activeAlerts.RemoveAll(e=>e.Alert._id==item.Alert._id);
                                break;
                            }
                        case AlertAction.ChangeState: {
                                var activeAlert = this._activeAlerts.FirstOrDefault(e => e.Alert._id == item.Alert._id);
                                if (activeAlert != null) {
                                    activeAlert.Alert.CurrentState = item.Alert.CurrentState;
                                    activeAlert.Reading = item.Reading;
                                    activeAlert.AlertAction = item.AlertAction;
                                } else {
                                    //Log Error
                                }
                                newStateTable.AddRow(item.Alert.displayName, item.Alert.CurrentState.ToString(), item.Reading.ToString());
                                //messageBuilder.AppendChanged(item.Alert.displayName, item.Alert.CurrentState.ToString(), item.Reading.ToString());
                                sendEmail = true;
                                break;
                            }
                        case AlertAction.Start: {
                                this._activeAlerts.Add(item.Clone());
                                sendEmail = true;
                                break;
                            }
                        case AlertAction.Resend: {
                                resendTable.AddRow(item.Alert.displayName, item.Alert.CurrentState.ToString(), item.Reading.ToString());
                                sendEmail = true;
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
                    messageBuilder.AppendStatus(item.Alert.displayName, item.Alert.CurrentState.ToString(), item.Reading.ToString());
                }
            }

            if (this._activeAlerts.Count > 0) {
                foreach (var active in this._activeAlerts) {
                    activeTable.AddRow(active.Alert.displayName, active.Alert.CurrentState.ToString(), active.Reading.ToString());
                    messageBuilder.AppendAlert(active.Alert.displayName, active.Alert.CurrentState.ToString(), active.Reading.ToString());
                }
                if (sendEmail) {
                    //var endpoint = await this._sendEnpoint.GetSendEndpoint(new Uri("rabbitmq://172.20.3.28:5672/email_processing"));
                    //await endpoint.Send<EmailContract>(new { Subject = "Epi2 Alerts", Message = messageBuilder.FinishMessage() });
                    await this._emailService.SendMessageAsync("Epi2 Alerts", messageBuilder.FinishMessage());
                }
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
        }
        private IEnumerable<ItemAlert> Process(IList<ItemAlert> itemAlerts) {
            var now = DateTime.Now;
            foreach (var itemAlert in itemAlerts) {
                var alertId = itemAlert.Alert._id;
                var actionType = itemAlert.Alert.CurrentState;
                var activeAlert = this._activeAlerts.FirstOrDefault(e => e.Alert._id == itemAlert.Alert._id);
                var actionItem = this._alertRepo.ActionItems.FirstOrDefault(e => e.actionType == actionType);                
                switch (itemAlert.Alert.CurrentState) {
                    case ActionType.Okay: {
                            if (activeAlert != null) {
                                itemAlert.AlertAction = AlertAction.Clear;
                                itemAlert.Alert.latched = false;
                                break;
                            } else {
                                itemAlert.AlertAction = AlertAction.ShowStatus;
                                break;
                            }
                        }
                    case ActionType.Alarm:
                    case ActionType.Warning:
                    case ActionType.SoftWarn: {
                            if (activeAlert != null) {
                                if (activeAlert.Alert.CurrentState != itemAlert.Alert.CurrentState) {
                                    itemAlert.Alert.lastAlarm = now;
                                    itemAlert.Alert.latched = true;
                                    itemAlert.AlertAction = AlertAction.ChangeState;
                                } else {
                                    if (actionItem != null) {
                                        if ((now - activeAlert.Alert.lastAlarm).TotalMinutes >= actionItem.EmailPeriod) {
                                            itemAlert.AlertAction = AlertAction.Resend;
                                            itemAlert.Alert.lastAlarm = now;
                                        } else {
                                            itemAlert.AlertAction = AlertAction.RemainActive;
                                        }
                                    } else {
                                        itemAlert.AlertAction = AlertAction.RemainActive;
                                        //this._logger.LogError($"ActionItem not found: {actionType.ToString()}");
                                    }
                                }

                            } else {
                                itemAlert.AlertAction = AlertAction.Start;
                                itemAlert.Alert.lastAlarm = now;
                            }
                            break;
                        }
                    case ActionType.Maintenance:
                    case ActionType.Custom:
                    default:
                        itemAlert.AlertAction = AlertAction.ShowStatus;
                        break;
                }
                yield return itemAlert;
            }
        }   
        public async Task Initialize() {
            await this._alertRepo.Load();
        }
    }
}
