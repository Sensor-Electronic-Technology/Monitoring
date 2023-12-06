using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MonitoringData.Infrastructure.Data;
using MonitoringData.Infrastructure.Events;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Services;
using MonitoringData.Infrastructure.Services.DataAccess;
using MonitoringSystem.Shared.SignalR;
using MonitoringData.Infrastructure.Utilities;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringData.Infrastructure.Services.DataLogging {
    public class ModbusLogger : IDataLogger {
        private const double fweight=.1;
        private readonly IMonitorDataRepo _dataService;
        private readonly IModbusService _modbusService;
        private readonly IHubContext<MonitorHub, IMonitorHub> _monitorHub;
        private readonly IAlertService _alertService;
        private readonly ILogger _logger;
        private Dictionary<ObjectId, double> _filters = new Dictionary<ObjectId, double>();

        private bool loggingEnabled = false;
        private bool _isAmmonia = false;
        private bool _recordData = true;
        private ManagedDevice _device;
        private IList<AlertRecord> _alerts;
        private DateTime lastRecord;
        private bool firstRecord;
        private TimeSpan _warmUp = new TimeSpan(0, 10, 0);

        public ModbusLogger(IMonitorDataRepo dataService,
            ILogger<ModbusLogger> logger,
            IAlertService alertService,
            IModbusService modbusService,
            IHubContext<MonitorHub,IMonitorHub> monitorHub) {
            this._dataService = dataService;
            this._modbusService = modbusService;
            this._monitorHub = monitorHub;
            this._alertService = alertService;
            this._logger = logger;
            this.loggingEnabled=true;
            this.firstRecord = true;
        }
        
        public async Task Read() {
            var result = await this._modbusService.Read(this._device.IpAddress, this._device.Port, 
                this._device.ModbusConfiguration);
            if (this._isAmmonia) {
                this._recordData = !(await this._modbusService.ReadCoil(this._device.IpAddress, 
                    this._device.Port, 1, 1));
            }
            if (result.Success) {
                var now = DateTime.Now;
                this._alerts = new List<AlertRecord>();
                if (result.HoldingRegisters != null) {
                    var analogRaw = new ArraySegment<ushort>(result.HoldingRegisters, this._device.ChannelMapping.AnalogStart, 
                        (this._device.ChannelMapping.AnalogStop - this._device.ChannelMapping.AnalogStart) + 1).ToArray();
                    if (this._device.DeviceName == "nh3") {
                        var tankScale=this._dataService.TankScales
                            .FirstOrDefault(e => e.TankScaleState == TankScaleState.InUse);
                        if (tankScale?.ChannelName != null) {
                            await this.ProcessNh3AnalogReadings(analogRaw, now, tankScale.ChannelName);
                        } else {
                            await this.ProcessAnalogReadings(analogRaw, now);
                        }
                    } else {
                        await this.ProcessAnalogReadings(analogRaw, now);
                    }
                    MonitorData monitorData = new MonitorData();
                    monitorData.TimeStamp = now;
                    monitorData.analogData = this._alerts
                        .Select(e => new ItemStatus() { 
                            Item=e.DisplayName,
                            State=e.CurrentState.ToString(),
                            Value= e.ChannelReading.ToString("N1")
                        }).ToList();
                    await this._alertService.ProcessAlerts(this._alerts,now);
                }
            } else {
                this.LogError("Modbus read failed");
            }
        }

        private async Task ProcessNh3AnalogReadings(ushort[] raw, DateTime now,string channelName) {
            List<AnalogReading> readings = new List<AnalogReading>();
            WeightReading weightReading=new WeightReading() {
                _id = ObjectId.GenerateNewId(),
                timestamp = now,
                ChannelName = channelName
            };
            foreach(var aItem in _dataService.AnalogItems) {
                var reading = new AnalogReading();
                reading.MonitorItemId = aItem._id;
                double tempValue=0;
                if (aItem.RegisterLength == 2) {
                    tempValue = Converters.ToInt32(raw[aItem.Register], raw[aItem.Register + 1])/(double)aItem.Factor;
                } else {
                    tempValue= raw[aItem.Register]/(double)aItem.Factor;
                }
                this._filters[aItem._id]+=(tempValue-this._filters[aItem._id])*fweight;
                reading.Value = this._filters[aItem._id];
                //reading.Value = tempValue;
                readings.Add(reading);
                if (aItem.Identifier == weightReading.ChannelName) {
                    weightReading.Value = reading.Value;
                }
                var alert=_dataService.MonitorAlerts.FirstOrDefault(e => e.ChannelId == aItem._id);
                if (alert != null) {
                    ActionType state=ActionType.Okay;
                    if ((int)reading.Value <= aItem.Level3SetPoint) {
                        state = aItem.Level3Action;
                    } else if ((int)reading.Value <= aItem.Level2SetPoint) {
                        state = aItem.Level2Action;
                    } else if ((int)reading.Value <= aItem.Level1SetPoint) {
                        state = aItem.Level1Action;
                    }
                    _alerts.Add(new AlertRecord(alert,(float)reading.Value,state));
                } else {
                    LogError($"AnalogChannel: {aItem.Identifier} alert not found");
                } 
            }
            if (CheckSave(now, lastRecord) && this._recordData) {
                lastRecord = now;
                await this._dataService.InsertWeightReading(weightReading);
                await _dataService.InsertOneAsync(new AnalogReadings {
                    readings=readings.ToArray(),
                    timestamp=now
                });
            }
        }

        private async Task ProcessAnalogReadings(ushort[] raw, DateTime now) {
            List<AnalogReading> readings = new List<AnalogReading>();

            foreach(var aItem in _dataService.AnalogItems) {
                var reading = new AnalogReading();
                reading.MonitorItemId = aItem._id;
                double tempValue=0;
                if (aItem.RegisterLength == 2) {
                    tempValue = Converters.ToInt32(raw[aItem.Register], raw[aItem.Register + 1])/(double)aItem.Factor;
                } else {
                    tempValue= raw[aItem.Register]/(double)aItem.Factor;
                }
                this._filters[aItem._id]+=(tempValue-this._filters[aItem._id])*fweight;
                reading.Value = this._filters[aItem._id];
                //reading.Value = tempValue;
                readings.Add(reading);

                var alert=_dataService.MonitorAlerts.FirstOrDefault(e => e.ChannelId == aItem._id);
                if (alert != null) {
                    ActionType state=ActionType.Okay;
                    if ((int)reading.Value <= aItem.Level3SetPoint) {
                        state = aItem.Level3Action;
                    } else if ((int)reading.Value <= aItem.Level2SetPoint) {
                        state = aItem.Level2Action;
                    } else if ((int)reading.Value <= aItem.Level1SetPoint) {
                        state = aItem.Level1Action;
                    }
                    _alerts.Add(new AlertRecord(alert,(float)reading.Value,state));
                } else {
                    LogError($"AnalogChannel: {aItem.Identifier} alert not found");
                } 
            }
            if (CheckSave(now, lastRecord) && this._recordData) {
                lastRecord = now;

                await _dataService.InsertOneAsync(new AnalogReadings {
                    readings=readings.ToArray(),
                    timestamp=now
                });
            }
        }
        
        private bool CheckSave(DateTime now,DateTime last) {
            if (this.firstRecord) {
                this.firstRecord = false;
                return true;
            } else {
                return ((now - last).TotalSeconds >= 60);
            }
        }

        public async Task Load() {
            await this._dataService.LoadAsync();
            await this._alertService.Load();
            this._device = this._dataService.ManagedDevice;
            if (this._device.DeviceName == "nh3") {
                this._isAmmonia = true;
            }
            var analogReading = await this._dataService.GetLastAnalogReading();
            foreach (var dev in this._dataService.AnalogItems) {
                if (analogReading != null) {
                    var reading = analogReading.readings.FirstOrDefault(e => e.MonitorItemId == dev._id);
                    if (reading != null) {
                        this._filters.Add(dev._id,reading.Value);
                    } else {
                        this._filters.Add(dev._id,0);
                    }
                } else {
                    this._filters.Add(dev._id,0);
                }
            }
        }

        public async Task Reload() {
            await this._dataService.ReloadAsync();
            await this._alertService.Reload();
            this._device = this._dataService.ManagedDevice;
        }

        public async Task Consume(ConsumeContext<ReloadConsumer> context) {
            this.LogInformation("Reloading System");
            await this.Load();
            this.LogInformation("Reload Finshed");
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
    }
}
