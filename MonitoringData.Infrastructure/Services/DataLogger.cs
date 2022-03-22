using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Services {



    public interface IDataLogger {
        Task Read();
        Task Load();
    }

    public class ModbusDataLogger : IDataLogger {
        private readonly IMonitorDataService _dataService;
        private IAlertService _alertService;
        private readonly ILogger _logger;
        private readonly FacilityContext _context;

        private NetworkConfiguration _networkConfig;
        private ModbusConfig _modbusConfig;
        private ChannelRegisterMapping _channelMapping;

        private bool initialized = false;

        private IList<ItemAlert> _itemAlerts;

        public ModbusDataLogger(IMonitorDataService dataService,ILogger logger,FacilityContext context) {
            this._dataService = dataService;
            this._logger = logger;
            this._context = context;
        }

        public ModbusDataLogger(IMonitorDataService dataService,FacilityContext context) {
            this._dataService = dataService;
            this._context = context;
        }
        public async Task Read() {
            var result = ModbusService.Read(this._networkConfig.IPAddress, this._networkConfig.Port, this._modbusConfig).GetAwaiter().GetResult();
            var discreteRaw = new ArraySegment<bool>(result.DiscreteInputs, this._channelMapping.DiscreteStart, (this._channelMapping.DiscreteStop - this._channelMapping.DiscreteStart) + 1).ToArray();
            var outputsRaw = new ArraySegment<bool>(result.DiscreteInputs, this._channelMapping.OutputStart, (this._channelMapping.OutputStop - this._channelMapping.OutputStart) + 1).ToArray();
            var actionsRaw = new ArraySegment<bool>(result.DiscreteInputs, this._channelMapping.ActionStart, (this._channelMapping.ActionStop - this._channelMapping.ActionStart) + 1).ToArray();
            var analogRaw = new ArraySegment<ushort>(result.InputRegisters, this._channelMapping.AnalogStart, (this._channelMapping.AnalogStop - this._channelMapping.AnalogStart) + 1).ToArray();
            var alertsRaw = new ArraySegment<ushort>(result.HoldingRegisters, this._channelMapping.AlertStart, (this._channelMapping.AlertStop - this._channelMapping.AlertStart) + 1).ToArray();
            var virtualRaw = new ArraySegment<bool>(result.Coils, this._channelMapping.VirtualStart, (this._channelMapping.VirtualStop - this._channelMapping.VirtualStart) + 1).ToArray();
            var now = DateTime.Now;
            this._itemAlerts = new List<ItemAlert>();
            if (result.HoldingRegisters.Length > this._channelMapping.DeviceStart - 1) {
                var deviceRaw = this.ToDeviceState(result.HoldingRegisters[this._channelMapping.DeviceStart]);
                await this._dataService.InsertDeviceReadingAsync(new DeviceReading() {
                    itemid = 1,
                    timestamp = now,
                    value = deviceRaw
                });
            } else {
                //Log Error
            }
            await this.ProcessAlertReadings(alertsRaw,now);
            await this.ProcessAnalogReadings(analogRaw,now);
            await this.ProcessDiscreteReadings(discreteRaw, now);
            await this.ProcessVirtualReadings(virtualRaw, now);
            await this.ProcessOutputReadings(outputsRaw, now);
            await this.ProcessActionReadings(actionsRaw, now);
            await this._alertService.ProcessAlerts(this._itemAlerts);
        }

        public async Task Load() {
            await this._dataService.LoadAsync();
            this._alertService = new AlertService(this._dataService);
            var device = await this._context.Devices.AsNoTracking()
                .OfType<ModbusDevice>()
                .FirstOrDefaultAsync(e => e.Identifier == "epi2");
            if (device != null) {
                this.initialized = true;
                this._networkConfig = device.NetworkConfiguration;
                this._modbusConfig = this._networkConfig.ModbusConfig;
                this._channelMapping = this._modbusConfig.ChannelMapping;
                Console.WriteLine("Loading Completed");
            } else {
                Console.WriteLine("Error:Device not found");
                this.initialized = false;
            }
        }

        private async Task ProcessAlertReadings(ushort[] raw,DateTime now) {

            List<AlertReading> alertReadings = new List<AlertReading>();
            for (int i = 0; i < raw.Length; i++) {
                var alertReading = new AlertReading() {
                    itemid = this._dataService.MonitorAlerts[i]._id,
                    timestamp = now,
                    value = this.ToActionType(raw[i])
                };
                alertReadings.Add(alertReading);
                this._itemAlerts.Add(new ItemAlert() {
                    Alert = this._dataService.MonitorAlerts[i],
                    AlertReading=alertReading,
                    Reading=0.00f
                });
            }
            await this._dataService.InsertManyAsync(alertReadings);
        }

        private async Task ProcessAnalogReadings(ushort[] raw, DateTime now) {
            if (this._dataService.AnalogItems.Count == raw.Length) {
                List<AnalogReading> analogReadings = new List<AnalogReading>();
                for (int i = 0; i < raw.Length; i++) {
                    var analogReading = new AnalogReading() { itemid = this._dataService.AnalogItems[i]._id, timestamp = now, value = raw[i] / 10 };
                    var alert = this._dataService.MonitorAlerts.FirstOrDefault(e => e._id == analogReading.itemid);
                    analogReadings.Add(analogReading);
                    var itemAlert = this._itemAlerts.FirstOrDefault(e => e.Alert.channelId == analogReading.itemid);
                    if (itemAlert != null) {
                        itemAlert.Reading = (float)analogReading.value;
                    } else {
                        //Log Error
                    }
                }
                await this._dataService.InsertManyAsync(analogReadings);
            } else {
                //Log Error
            }
        }

        private async Task ProcessDiscreteReadings(bool[] raw,DateTime now) {
            if (this._dataService.DiscreteItems.Count == raw.Length) {
                List<DiscreteReading> readings = new List<DiscreteReading>();
                for (int i = 0; i < raw.Length; i++) {
                    var reading = new DiscreteReading() {
                        itemid = this._dataService.DiscreteItems[i]._id,
                        timestamp = now,
                        value = raw[i]
                    };
                    readings.Add(reading);
                    var alert = this._dataService.MonitorAlerts.FirstOrDefault(e => e._id == reading.itemid);
                    var itemAlert = this._itemAlerts.FirstOrDefault(e => e.Alert.channelId == reading.itemid);
                    if (itemAlert != null) {
                        itemAlert.Reading = reading.value == true ? 1.00f : 0.00f;
                    } else {
                        //Log Error
                    }
                }
                await this._dataService.InsertManyAsync(readings);
            } else {
                //Log Error
            }
        }

        private async Task ProcessVirtualReadings(bool[] raw, DateTime now) {
            if (this._dataService.VirtualItems.Count == raw.Length) {
                List<VirtualReading> readings = new List<VirtualReading>();
                for (int i = 0; i < raw.Length; i++) {
                    var reading = new VirtualReading() {
                        itemid = this._dataService.VirtualItems[i]._id,
                        timestamp = now,
                        value = raw[i]
                    };
                    var alert = this._dataService.MonitorAlerts.FirstOrDefault(e => e._id == reading.itemid);
                    readings.Add(reading);
                    var itemAlert = this._itemAlerts.FirstOrDefault(e => e.Alert.channelId == reading.itemid);
                    if (itemAlert != null) {
                        itemAlert.Reading = reading.value == true ? 1.00f : 0.00f;
                    } else {
                        //Log Error
                    }
                }
                await this._dataService.InsertManyAsync(readings);
            } else {
                //Log Error
            }

        }

        private async Task ProcessOutputReadings(bool[] raw,DateTime now) {
            if (this._dataService.OutputItems.Count == raw.Length) {
                List<OutputReading> readings = new List<OutputReading>();
                for (int i = 0; i < raw.Length; i++) {
                    readings.Add(new OutputReading() { itemid = this._dataService.OutputItems[i]._id, timestamp = now, value = raw[i] });
                }
                await this._dataService.InsertManyAsync(readings);
            } else {
                //Log Error
            }
        }

        private async Task ProcessActionReadings(bool[] raw,DateTime now) {
            if (this._dataService.ActionItems.Count == raw.Length) {
                List<ActionReading> readings = new List<ActionReading>();
                for (int i = 0; i < raw.Length; i++) {
                    readings.Add(new ActionReading() {
                        itemid = this._dataService.ActionItems[i]._id,
                        timestamp = now,
                        value = raw[i]
                    });
                }
                await this._dataService.InsertManyAsync(readings);
            } else {
                //Log Error
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
    }

}
