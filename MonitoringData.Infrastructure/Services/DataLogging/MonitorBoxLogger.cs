using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MonitoringData.Infrastructure.Data;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringData.Infrastructure.Services.DataAccess;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;
using MonitoringSystem.Shared.Services;

namespace MonitoringData.Infrastructure.Services.DataLogging {
    public class MonitorBoxLogger : IDataLogger {
        private readonly IMonitorDataRepo _dataService;
        private readonly IModbusService _modbusService;
        private readonly IAlertService _alertService;
        private readonly ILogger _logger;
        private ManagedDevice _device;
        private bool _loggingEnabled = false;
        private IList<AlertRecord> _alerts;
        private DateTime _lastRecord;
        private TimeSpan _recordInterval;
        private bool _firstRecord;
        private DeviceCheck _deviceCheck=new DeviceCheck();
        private Dictionary<ObjectId, bool> _lastState = new Dictionary<ObjectId, bool>();
        
        public MonitorBoxLogger(IMonitorDataRepo dataService,
            ILogger<MonitorBoxLogger> logger, 
            IAlertService alertService,
            IModbusService modbusService) {
            this._dataService = dataService;
            this._alertService = alertService;
            this._logger = logger;
            this._modbusService = modbusService;
            this._loggingEnabled = true;
            this._firstRecord = true;
        }
        
        public async Task Read() {
            var result = await this._modbusService.Read(this._device.IpAddress!, 
                this._device.Port, 
                this._device.ModbusConfiguration!);
            var now = DateTime.Now;
            if (result.Success) {
                this._deviceCheck.Clear();
                var discreteRaw = new ArraySegment<bool>(result.DiscreteInputs, this._device.ChannelMapping!.DiscreteStart,
                    (this._device.ChannelMapping!.DiscreteStop - this._device.ChannelMapping!.DiscreteStart) + 1).ToArray();
                var analogRaw = new ArraySegment<ushort>(result.InputRegisters, this._device.ChannelMapping!.AnalogStart, 
                    (this._device.ChannelMapping.AnalogStop - this._device.ChannelMapping!.AnalogStart) + 1).ToArray();
                var alertsRaw = new ArraySegment<ushort>(result.HoldingRegisters, this._device.ChannelMapping!.AlertStart,
                    (this._device.ChannelMapping!.AlertStop - this._device.ChannelMapping!.AlertStart) + 1).ToArray();
                var virtualRaw = new ArraySegment<bool>(result.Coils, this._device.ChannelMapping.VirtualStart, 
                    (this._device.ChannelMapping!.VirtualStop - this._device.ChannelMapping!.VirtualStart) + 1).ToArray();
                this._alerts = new List<AlertRecord>();
                await this.ProcessAlertReadings(alertsRaw);
                var analogProcessed=await this.ProcessAnalogReadings(analogRaw, now);
                var discreteProcessed=await this.ProcessDiscreteReadings(discreteRaw, now);
                var virtualProcessed=await this.ProcessVirtualReadings(virtualRaw, now);
                if(CheckSave(now,(analogProcessed.Item2 || discreteProcessed.Item2 || virtualProcessed.Item2))) {
                    this._lastRecord = now;
                    await this._dataService.InsertOneAsync(analogProcessed.Item1);
                    await this._dataService.InsertOneAsync(discreteProcessed.Item1);
                    await this._dataService.InsertOneAsync(virtualProcessed.Item1);
                    this.LogInformation("Data Recorded");
                }
                this._logger.LogInformation("Data Read");
                await this._alertService.ProcessAlerts(this._alerts,now);
            } else {
                if (this._deviceCheck.CheckTime(now)) {
                    await this._alertService.DeviceOfflineAlert();
                    this.LogInformation("Device Offline Email Sent");
                }
                this._logger.LogError("Modbus read failed");
            }
        }

        private Task ProcessAlertReadings(ushort[] raw) {
            foreach (var alert in this._dataService.MonitorAlerts) {
                var alertReading = new AlertReading() {
                    MonitorItemId = alert._id,
                    AlertState = this.ToActionType(raw[alert.Register])
                };
                this._alerts.Add(new AlertRecord(alert,alertReading.AlertState));
            }
            return Task.CompletedTask;
        }
        
        private Task<Tuple<AnalogReadings?, bool>> ProcessAnalogReadings(ushort[] raw, DateTime now) {
            if (this._dataService.AnalogItems.Count == raw.Length) {
                List<AnalogReading> readings = new List<AnalogReading>();
                bool record = false;
                foreach (var item in this._dataService.AnalogItems) {
                    var analogReading = new AnalogReading() {
                        MonitorItemId = item._id, Value = (float)raw[item.Register] / item.Factor
                    };
                    if (item.Connected) {
                        var thresholdMet=(item.ValueDirection==ValueDirection.Increasing) ? 
                            (analogReading.Value >= item.RecordThreshold):(analogReading.Value<=item.RecordThreshold);
                        if (thresholdMet && ((now - this._lastRecord).TotalSeconds >= item.ThresholdInterval)) {
                            record = true;
                        }
                    }
                    readings.Add(analogReading);
                    var alertRecord = this._alerts.FirstOrDefault(e => e.ChannelId == item._id);
                    if (alertRecord != null) {
                        alertRecord.ChannelReading = (float)analogReading.Value;
                    } else {
                        this.LogError("Analog MonitorAlert not found");
                    }
                }
                return Task.FromResult<Tuple<AnalogReadings?, bool>>(new(
                    new AnalogReadings() { 
                        readings = readings.ToArray(), 
                        timestamp = now 
                    }, record));
            } else {
                this.LogError("Error: AnalogItems count doesn't match raw data count");
                return Task.FromResult<Tuple<AnalogReadings?, bool>>(new(null,false));
            }
        }

        private Task<Tuple<DiscreteReadings?, bool>> ProcessDiscreteReadings(bool[] raw, DateTime now) {
            bool record = false;
            if (this._dataService.DiscreteItems.Count == raw.Length) {
                List<DiscreteReading> readings = new List<DiscreteReading>();
                foreach (var item in this._dataService.DiscreteItems) {
                    var reading = new DiscreteReading() {
                        MonitorItemId = item._id,
                        Value = raw[item.Register]
                    };
                    if (this._firstRecord) {
                        this._lastState[item._id] = reading.Value;
                        record = item.Connected;
                    } else {
                        record = item.Connected && (reading.Value != this._lastState[item._id]);
                    }
                    readings.Add(reading);
                    var alertRecord = this._alerts.FirstOrDefault(e => e.ChannelId == item._id);
                    if (alertRecord != null) {
                        alertRecord.ChannelReading = reading.Value ? 1.00f : 0.00f;
                    } else {
                        this.LogError("Discrete ItemAlert not found");
                    }
                }
                return Task.FromResult<Tuple<DiscreteReadings?, bool>>(new(
                    new DiscreteReadings() { 
                        readings = readings.ToArray(), 
                        timestamp = now }, 
                    record));
            } else {
                this.LogError("Error: DiscreteItems count doesn't match raw data count");
                return Task.FromResult<Tuple<DiscreteReadings?, bool>>(new(null, false));
            }
        }

        private Task<Tuple<VirtualReadings?, bool>> ProcessVirtualReadings(bool[] raw, DateTime now) {         
            if (this._dataService.VirtualItems.Count == raw.Length) {
                bool record = false;
                List<VirtualReading> readings = new List<VirtualReading>();
                foreach (var item in this._dataService.VirtualItems) {
                    var reading = new VirtualReading() {
                        MonitorItemId = item._id,
                        Value = raw[item.Register]
                    };
                    record = item.Connected && reading.Value &&
                             ((now - this._lastRecord).TotalSeconds >= item.ThresholdInterval);
                    
                    readings.Add(reading);
                    var alertRecord = this._alerts.FirstOrDefault(e => e.ChannelId == item._id);
                    if (alertRecord != null) {
                        alertRecord.ChannelReading = reading.Value ? 1.00f : 0.00f;
                    } else {
                        this.LogError("Virtual ItemAlert not found");
                    }
                }
                return Task.FromResult<Tuple<VirtualReadings?, bool>>(new(
                    new VirtualReadings() { 
                        readings = readings.ToArray(), 
                        timestamp = now }, 
                    record));
            } else {
                this.LogError("Error: VirtualItems count doesn't match raw data count");
                return Task.FromResult<Tuple<VirtualReadings?, bool>>(new(null, false));
            }
        }

        private bool CheckSave(DateTime now,bool thresholdMet) {
            if (this._firstRecord) {
                this._firstRecord = false;
                return true;
            } else {
                var deltaTime = (now - this._lastRecord).TotalSeconds;
                return (deltaTime >= this._recordInterval.TotalSeconds) || thresholdMet;
            }
        }

        private void LogInformation(string msg) {
            if (this._loggingEnabled) {
                this._logger.LogInformation(msg);
            } else {
                Console.WriteLine(msg);
            }
        }

        private void LogWarning(string msg) {
            if (this._loggingEnabled) {
                this._logger.LogWarning(msg);
            } else {
                Console.WriteLine(msg);
            }
        }

        private void LogError(string msg) {
            if (this._loggingEnabled) {
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
        public async Task Load() {
            await this._dataService.LoadAsync();
            await this._alertService.Load();
            this._device = this._dataService.ManagedDevice;
            this._recordInterval = new TimeSpan(0, 0, this._device.RecordInterval);
            foreach (var item in this._dataService.DiscreteItems) {
                this._lastState.Add(item._id,false);
            }
        }
        public async Task Reload() {
            this.LogInformation("Reloading DataLogger");
            await this._dataService.ReloadAsync();
            await this._alertService.Reload();
            this._device = this._dataService.ManagedDevice;
            this._recordInterval = new TimeSpan(0, 0, this._device.RecordInterval);
            this._firstRecord = true;
            this._lastState.Clear();
            foreach (var item in this._dataService.DiscreteItems) {
                this._lastState.Add(item._id, false);
            }
        }
    }
    
}
