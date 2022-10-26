﻿using ConsoleTables;
using Microsoft.Extensions.Logging;
using MonitoringData.Infrastructure.Data;
using MonitoringSystem.Shared.Data;
using MonitoringData.Infrastructure.Services.DataAccess;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;
using MonitoringSystem.Shared.Services;

namespace MonitoringData.Infrastructure.Services {
    public class MonitorBoxLogger : IDataLogger {
        private readonly IMonitorDataRepo _dataService;
        private readonly IModbusService _modbusService;
        private readonly IAlertService _alertService;
        private readonly ILogger _logger;

        private ManagedDevice _device;
        private bool loggingEnabled = false;
        private IList<AlertRecord> _alerts;

        private DateTime _lastRecord;
        private TimeSpan _recordInterval;
        private bool _firstRecord;

        private DateTime _boxOfflineTime;
        private bool _offlineLatch = false;
        private readonly int _alertTime = 60;
        
        public MonitorBoxLogger(IMonitorDataRepo dataService,
            ILogger<MonitorBoxLogger> logger, 
            IAlertService alertService,
            IModbusService modbusService) {
            this._dataService = dataService;
            this._alertService = alertService;
            this._logger = logger;
            this._modbusService = modbusService;
            this.loggingEnabled = true;
            this._firstRecord = true;
        }
        
        public async Task Read() {
            var result = await this._modbusService.Read(this._device.IpAddress, this._device.Port, 
                this._device.ModbusConfiguration);
            var now = DateTime.Now;
            if (result.Success) {
                this._offlineLatch = false;
                var discreteRaw = new ArraySegment<bool>(result.DiscreteInputs, this._device.ChannelMapping.DiscreteStart,
                    (this._device.ChannelMapping.DiscreteStop - this._device.ChannelMapping.DiscreteStart) + 1).ToArray();
                var analogRaw = new ArraySegment<ushort>(result.InputRegisters, this._device.ChannelMapping.AnalogStart, 
                    (this._device.ChannelMapping.AnalogStop - this._device.ChannelMapping.AnalogStart) + 1).ToArray();
                var alertsRaw = new ArraySegment<ushort>(result.HoldingRegisters, this._device.ChannelMapping.AlertStart,
                    (this._device.ChannelMapping.AlertStop - this._device.ChannelMapping.AlertStart) + 1).ToArray();
                var virtualRaw = new ArraySegment<bool>(result.Coils, this._device.ChannelMapping.VirtualStart, 
                    (this._device.ChannelMapping.VirtualStop - this._device.ChannelMapping.VirtualStart) + 1).ToArray();
                this._alerts = new List<AlertRecord>();
                await this.ProcessAlertReadings(alertsRaw, now);
                var aret=await this.ProcessAnalogReadings(analogRaw, now);
                var dret=await this.ProcessDiscreteReadings(discreteRaw, now);
                var vret=await this.ProcessVirtualReadings(virtualRaw, now);
                
                if(CheckSave(now,this._lastRecord,(aret.Item2 || dret.Item2 || vret.Item2))) {
                    this._lastRecord = now;
                    if (aret.Item1 != null) {
                        await this._dataService.InsertOneAsync(aret.Item1);
                    }
                    if (dret.Item1 != null) {
                        await this._dataService.InsertOneAsync(dret.Item1);
                    }
                    if (vret.Item1 != null) {
                        await this._dataService.InsertOneAsync(vret.Item1);
                    }
                    this.LogInformation("Data Recorded");
                }
                await this._alertService.ProcessAlerts(this._alerts,now);
            } else {
                if (!this._offlineLatch) {
                    this._offlineLatch = true;
                    this._boxOfflineTime = now;
                } else {
                    if((now - this._boxOfflineTime).TotalSeconds >= 60) {
                        await this._alertService.DeviceOfflineAlert();
                        this._boxOfflineTime = now;
                    }
                }
                this._logger.LogError("Modbus read failed");
            }
        }

        private Task ProcessAlertReadings(ushort[] raw, DateTime now) {
            List<AlertReading> alertReadings = new List<AlertReading>();
            /*ConsoleTable table = new ConsoleTable("Name", "Register", "Value");*/
            foreach (var alert in this._dataService.MonitorAlerts) {
                var alertReading = new AlertReading() {
                    MonitorItemId = alert._id,
                    AlertState = this.ToActionType(raw[alert.Register])
                };
                alertReadings.Add(alertReading);
                this._alerts.Add(new AlertRecord(alert,alertReading.AlertState));
                /*table.AddRow(alert.DisplayName, alert.Register,alertReading.AlertState.ToString());*/
            }
            /*Console.WriteLine(table.ToMinimalString());*/
            return Task.CompletedTask;
        }

        private Task<Tuple<AnalogReadings,bool>> ProcessAnalogReadings(ushort[] raw, DateTime now) {
            if (this._dataService.AnalogItems.Count == raw.Length) {
                List<AnalogReading> readings = new List<AnalogReading>();
                bool record = false;
                foreach (var item in this._dataService.AnalogItems) {
                    var analogReading = new AnalogReading() {
                        MonitorItemId = item._id, Value = (float)raw[item.Register] / item.Factor
                    };
                    if (analogReading.Value >= item.RecordThreshold && item.Connected) {
                        record = true;
                    }
                    readings.Add(analogReading);
                    var alertRecord = this._alerts.FirstOrDefault(e => e.ChannelId == item._id);
                    if (alertRecord != null) {
                        alertRecord.ChannelReading = (float)analogReading.Value;
                    } else {
                        this.LogError("Analog MonitorAlert not found");
                    }
                }
                return Task.FromResult<Tuple<AnalogReadings, bool>>(new(
                    new AnalogReadings() { 
                        readings = readings.ToArray(), 
                        timestamp = now 
                    }, record));
            } else {
                this.LogError("Error: AnalogItems count doesn't match raw data count");
                return Task.FromResult<Tuple<AnalogReadings, bool>>(new(null,false));
            }
        }

        private Task<Tuple<DiscreteReadings,bool>> ProcessDiscreteReadings(bool[] raw, DateTime now) {
            bool record = false;
            if (this._dataService.DiscreteItems.Count == raw.Length) {
                List<DiscreteReading> readings = new List<DiscreteReading>();
                foreach (var item in this._dataService.DiscreteItems) {
                    var reading = new DiscreteReading() {
                        MonitorItemId = item._id,
                        Value = raw[item.Register]
                    };
                    if (reading.Value && item.Connected) {
                        record = true;
                    }              
                    readings.Add(reading);
                    var alertRecord = this._alerts.FirstOrDefault(e => e.ChannelId == item._id);
                    if (alertRecord != null) {
                        alertRecord.ChannelReading = reading.Value == true ? 1.00f : 0.00f;
                    } else {
                        this.LogError("Discrete ItemAlert not found");
                    }
                }
                return Task.FromResult<Tuple<DiscreteReadings, bool>>(new(
                    new DiscreteReadings() { readings = readings.ToArray(), timestamp = now }, 
                    record));
            } else {
                this.LogError("Error: DiscreteItems count doesn't match raw data count");
                return Task.FromResult<Tuple<DiscreteReadings, bool>>(new(null, false));
            }
        }

        private Task<Tuple<VirtualReadings,bool>> ProcessVirtualReadings(bool[] raw, DateTime now) {         
            if (this._dataService.VirtualItems.Count == raw.Length) {
                bool record = false;
                List<VirtualReading> readings = new List<VirtualReading>();
                foreach (var item in this._dataService.VirtualItems) {
                    var reading = new VirtualReading() {
                        MonitorItemId = item._id,
                        Value = raw[item.Register]
                    };
                    if(reading.Value && item.Connected) {
                        record = true;
                    }
                    readings.Add(reading);
                    var alertRecord = this._alerts.FirstOrDefault(e => e.ChannelId == item._id);
                    if (alertRecord != null) {
                        alertRecord.ChannelReading = reading.Value == true ? 1.00f : 0.00f;
                    } else {
                        this.LogError("Virtual ItemAlert not found");
                    }
                }
                return Task.FromResult<Tuple<VirtualReadings, bool>>(new(
                    new VirtualReadings() { 
                        readings = readings.ToArray(), 
                        timestamp = now }, 
                    record));
            } else {
                this.LogError("Error: Virtualitems count doesn't match raw data count");
                return Task.FromResult<Tuple<VirtualReadings, bool>>(new(null, false));
            }
        }

        private bool CheckSave(DateTime now,DateTime last,bool thresholdMet) {
            if (this._firstRecord) {
                this._firstRecord = false;
                return true;
            } else {
                return ((now - last).TotalSeconds >= this._recordInterval.TotalSeconds)
                        || thresholdMet;
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
        public async Task Load() {
            await this._dataService.LoadAsync();
            await this._alertService.Load();
            this._device = this._dataService.ManagedDevice;
            this._recordInterval = new TimeSpan(0, 0, this._device.RecordInterval);
        }

        public async Task Reload() {
            await this._dataService.ReloadAsync();
            await this._alertService.Reload();
            this._device = this._dataService.ManagedDevice;
            this._recordInterval = new TimeSpan(0, 0, this._device.RecordInterval);
        }
        
        /*public async Task Consume(ConsumeContext<ReloadConsumer> context) {
            this.LogInformation("Reloading System");
            await this.Reload();
            this.LogInformation($"Device Ip: {this._device.IpAddress}");
            this.LogInformation("Reload Finished");
        }*/
    }

}
