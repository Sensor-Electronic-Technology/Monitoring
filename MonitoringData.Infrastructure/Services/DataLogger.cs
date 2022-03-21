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

    public enum AlertAction {
        Clear,
        Start,
        Resend,
        Nothing
    }

    public interface IDataLogger {
        Task Read();
        Task Load();
    }

    public class ModbusDataLogger : IDataLogger {

        private readonly IMonitorDataService _dataService;
        private readonly IAlertService _alertService;
        private readonly ILogger _logger;
        private readonly FacilityContext _context;

        private NetworkConfiguration _networkConfig;
        private ModbusConfig _modbusConfig;
        private ChannelRegisterMapping _channelMapping;

        private bool initialized = false;

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

            var itemAlerts = new List<ItemAlert>();

            List<AlertReading> alertReadingTemp = new List<AlertReading>();

            for (int i = 0; i < alertsRaw.Length; i++) {
                var alertReading = new AlertReading() {
                    itemid = this._dataService.MonitorAlerts[i]._id,
                    timestamp = now,
                    value = this.ToActionType(alertsRaw[i])
                };
                alertReadingTemp.Add(alertReading);
            }
            await this._dataService.InsertManyAsync(alertReadingTemp);

            List<AnalogReading> aTempReadings = new List<AnalogReading>();
            for (int i = 0; i < analogRaw.Length; i++) {
                var analogReading = new AnalogReading() { itemid = this._dataService.AnalogItems[i]._id, timestamp = now, value = analogRaw[i] / 10 };
                var alert = this._dataService.MonitorAlerts.FirstOrDefault(e => e._id == analogReading.itemid);
                aTempReadings.Add(analogReading);
                itemAlerts.Add(new ItemAlert() {
                    Alert = alert,
                    Reading = (float)analogReading.value
                });
            }
            await this._dataService.InsertManyAsync(aTempReadings);

            List<DiscreteReading> dTempReadings = new List<DiscreteReading>();
            for (int i = 0; i < discreteRaw.Length; i++) {
                dTempReadings.Add(new DiscreteReading() { itemid = this._dataService.DiscreteItems[i]._id, timestamp = now, value = discreteRaw[i] });
            }
            await this._dataService.InsertManyAsync(dTempReadings);

            List<OutputReading> oReadings = new List<OutputReading>();
            for (int i = 0; i < outputsRaw.Length; i++) {
                oReadings.Add(new OutputReading() { itemid = this._dataService.OutputItems[i]._id, timestamp = now, value = outputsRaw[i] });
            }
            await this._dataService.InsertManyAsync(oReadings);

            List<ActionReading> actReadingTemp = new List<ActionReading>();
            for (int i = 0; i < actionsRaw.Length; i++) {
                actReadingTemp.Add(new ActionReading() { itemid = this._dataService.ActionItems[i]._id, timestamp = now, value = actionsRaw[i] });
            }
            await this._dataService.InsertManyAsync(actReadingTemp);

            await this._dataService.InsertDeviceReadingAsync(new DeviceReading() {
                itemid = 1,
                timestamp = now,
                value =this.ToDeviceState(result.HoldingRegisters[this._channelMapping.DeviceStart])
            });
        }

        public async Task Load() {
            await this._dataService.LoadAsync();
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
