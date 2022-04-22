using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MonitoringData.Infrastructure.Events;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringData.Infrastructure.Services.DataAccess;
using MonitoringData.Infrastructure.Services.SignalR;
using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Services {
    public interface IDataLogger {
        Task Read();
        Task Load();
    }

    public class ModbusDataLogger : IDataLogger , IConsumer<ReloadConsumer> {
        private readonly IMonitorDataRepo _dataService;
        private readonly IModbusService _modbusService;
        private readonly IHubContext<MonitorHub, IMonitorHub> _monitorHub;
        private IAlertService _alertService;
        private readonly ILogger _logger;


        private NetworkConfiguration _networkConfig;
        private ModbusConfig _modbusConfig;
        private ChannelRegisterMapping _channelMapping;

        private bool loggingEnabled = false;
        private IList<AlertRecord> _alerts;

        public ModbusDataLogger(IMonitorDataRepo dataService,
            ILogger<ModbusDataLogger> logger, 
            IAlertService alertService,
            IModbusService modbusService,
            IHubContext<MonitorHub,IMonitorHub> monitorHub) {

            this._dataService = dataService;
            this._alertService = alertService;
            this._logger = logger;
            this._modbusService = modbusService;
            this.loggingEnabled = true;
        }

        public ModbusDataLogger(string connName, string databaseName, Dictionary<Type, string> collectionNames) {
            this._dataService = new MonitorDataService(connName, databaseName, collectionNames);
            this._alertService = new AlertService(connName, databaseName, collectionNames[typeof(ActionItem)], collectionNames[typeof(MonitorAlert)]);
            this.loggingEnabled = true;
            this._modbusService = new ModbusService();
        }

        public async Task Read() {
            var result = await this._modbusService.Read(this._networkConfig.IPAddress, this._networkConfig.Port, this._modbusConfig);
            if (result._success) {
                var discreteRaw = new ArraySegment<bool>(result.DiscreteInputs, this._channelMapping.DiscreteStart, (this._channelMapping.DiscreteStop - this._channelMapping.DiscreteStart) + 1).ToArray();
                var analogRaw = new ArraySegment<ushort>(result.InputRegisters, this._channelMapping.AnalogStart, (this._channelMapping.AnalogStop - this._channelMapping.AnalogStart) + 1).ToArray();
                var alertsRaw = new ArraySegment<ushort>(result.HoldingRegisters, this._channelMapping.AlertStart, (this._channelMapping.AlertStop - this._channelMapping.AlertStart) + 1).ToArray();
                var virtualRaw = new ArraySegment<bool>(result.Coils, this._channelMapping.VirtualStart, (this._channelMapping.VirtualStop - this._channelMapping.VirtualStart) + 1).ToArray();
                var now = DateTime.Now;
                this._alerts = new List<AlertRecord>();
                if (result.HoldingRegisters.Length > this._channelMapping.DeviceStart - 1) {
                    var deviceRaw = this.ToDeviceState(result.HoldingRegisters[this._channelMapping.DeviceStart]);
                    await this._dataService.InsertDeviceReadingAsync(new DeviceReading() {
                        itemid = 1,
                        timestamp = now,
                        value = deviceRaw
                    });
                } else {
                    this.LogError("Error: HoldingRegister count is < DeviceStart Address");
                }
                await this.ProcessAlertReadings(alertsRaw, now);
                await this.ProcessAnalogReadings(analogRaw, now);
                await this.ProcessDiscreteReadings(discreteRaw, now);
                await this.ProcessVirtualReadings(virtualRaw, now);
                var output=this._alerts
                    .Where(e => e.Enabled)
                    .Select(e => new ItemStatus() { 
                        Item = e.DisplayName, 
                        State = e.CurrentState.ToString(), 
                        Value = e.ChannelReading.ToString() 
                    }).ToList();
                await this._alertService.ProcessAlerts(this._alerts,now);
                await this._monitorHub.Clients.All.ShowCurrent(output);
            } else {
                this._logger.LogError("Modbus read failed");
            }
        }

        public async Task Load() {
            await this._dataService.LoadAsync();
            await this._alertService.Initialize();
            this._networkConfig = this._dataService.MonitorDevice.NetworkConfiguration;
            this._modbusConfig = this._networkConfig.ModbusConfig;
            this._channelMapping = this._modbusConfig.ChannelMapping;
        }

        private Task ProcessAlertReadings(ushort[] raw, DateTime now) {
            List<AlertReading> alertReadings = new List<AlertReading>();
            for (int i = 0; i < raw.Length; i++) {
                var alertReading = new AlertReading() {
                    itemid = this._dataService.MonitorAlerts[i]._id,
                    state = this.ToActionType(raw[i])
                };
                alertReadings.Add(alertReading);
                this._alerts.Add(new AlertRecord(this._dataService.MonitorAlerts[i], alertReading.state));
            }
            return Task.CompletedTask;
        }

        private async Task ProcessAnalogReadings(ushort[] raw, DateTime now) {
            if (this._dataService.AnalogItems.Count == raw.Length) {
                List<AnalogReading> readings = new List<AnalogReading>();
                for (int i = 0; i < raw.Length; i++) {
                    var analogReading = new AnalogReading() { 
                        itemid = this._dataService.AnalogItems[i]._id, 
                        value = (float)raw[i] / (float)this._dataService.AnalogItems[i].factor
                    };
                    readings.Add(analogReading);
                    var alertRecord = this._alerts.FirstOrDefault(e => e.ChannelId == analogReading.itemid);
                    if (alertRecord != null) {
                        alertRecord.ChannelReading = (float)analogReading.value;
                    } else {
                        this.LogError("Analog ItemAlert not found");
                    }
                }
                await this._dataService.InsertOneAsync(new AnalogReadings() { readings = readings.ToArray(), timestamp = now });
            } else {
                this.LogError("Error: AnalogItems count doesn't match raw data count");
            }
        }

        private async Task ProcessDiscreteReadings(bool[] raw, DateTime now) {
            if (this._dataService.DiscreteItems.Count == raw.Length) {
                List<DiscreteReading> readings = new List<DiscreteReading>();
                for (int i = 0; i < raw.Length; i++) {
                    var reading = new DiscreteReading() {
                        itemid = this._dataService.DiscreteItems[i]._id,
                        value = raw[i]
                    };
                    readings.Add(reading);
                    var alertRecord = this._alerts.FirstOrDefault(e => e.ChannelId == reading.itemid);
                    if (alertRecord != null) {
                        alertRecord.ChannelReading = reading.value == true ? 1.00f : 0.00f;
                    } else {
                        this.LogError("Discrete ItemAlert not found");
                    }
                }
                await this._dataService.InsertOneAsync(new DiscreteReadings() { readings = readings.ToArray(), timestamp = now });
            } else {
                this.LogError("Error: DiscreteItems count doesn't match raw data count");
            }
        }

        private async Task ProcessVirtualReadings(bool[] raw, DateTime now) {
            if (this._dataService.VirtualItems.Count == raw.Length) {
                List<VirtualReading> readings = new List<VirtualReading>();
                for (int i = 0; i < raw.Length; i++) {
                    var reading = new VirtualReading() {
                        itemid = this._dataService.VirtualItems[i]._id,
                        value = raw[i]
                    };
                    var alert = this._dataService.MonitorAlerts.FirstOrDefault(e => e._id == reading.itemid);
                    readings.Add(reading);
                    var alertRecord = this._alerts.FirstOrDefault(e => e.ChannelId == reading.itemid);
                    if (alertRecord != null) {
                        alertRecord.ChannelReading = reading.value == true ? 1.00f : 0.00f;
                    } else {
                        this.LogError("Virual ItemAlert not found");
                    }
                }
                await this._dataService.InsertOneAsync(new VirtualReadings() { readings = readings.ToArray(), timestamp = now });
            } else {
                this.LogError("Error: Virtualitems count doesn't match raw data count");
            }

        }

        private void LogInformation(string msg) {
            if (this.loggingEnabled) {
                this._logger.LogInformation(msg);
            } else {
                Console.WriteLine(msg);
            }
        }

        private void LogWarning(string msg) {
            if (this.loggingEnabled) {
                this._logger.LogWarning(msg);
            } else {
                Console.WriteLine(msg);
            }
        }

        private void LogError(string msg) {
            if (this.loggingEnabled) {
                this._logger.LogError(msg);
            } else {
                Console.WriteLine(msg);
            }
        }

        private ActionType ToActionType(ushort value) {
            switch (value) {
                case 1: {
                        return ActionType.Custom;
                    }
                case 2: {
                        return ActionType.Maintenance;
                    }
                case 3: {
                        return ActionType.SoftWarn;
                    }
                case 4: {
                        return ActionType.Warning;
                    }
                case 5: {
                        return ActionType.Alarm;
                    }
                case 6: {
                        return ActionType.Okay;
                    }
                default: {
                        return ActionType.Okay;
                    }
            }
        }

        private DeviceState ToDeviceState(ushort value) {
            switch (value) {
                case 0: {
                        return DeviceState.OKAY;
                    }
                case 1: {
                        return DeviceState.WARNING;
                    }
                case 2: {
                        return DeviceState.ALARM;
                    }
                case 3: {
                        return DeviceState.MAINTENANCE;
                    }
                default: {
                        return DeviceState.OKAY;
                    }
            }
        }

        public async Task Consume(ConsumeContext<ReloadConsumer> context) {
            this.LogInformation("Reloading System");
            await this.Load();
            this.LogInformation("Reload Finshed");
        }
    }

}
