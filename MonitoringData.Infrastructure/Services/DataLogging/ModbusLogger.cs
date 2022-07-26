using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MonitoringData.Infrastructure.Events;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Services;
using MonitoringData.Infrastructure.Services.DataAccess;
using MonitoringSystem.Shared.SignalR;
using MonitoringData.Infrastructure.Utilities;

namespace MonitoringData.Infrastructure.Services.DataLogging {
    public class ModbusLogger : IDataLogger, IConsumer<ReloadConsumer> {
        private readonly IMonitorDataRepo _dataService;
        private readonly IModbusService _modbusService;
        private readonly IHubContext<MonitorHub, IMonitorHub> _monitorHub;
        private IAlertService _alertService;
        private readonly ILogger _logger;

        private bool loggingEnabled = false;
        private ManagedDevice _device;
        private IList<AlertRecord> _alerts;
        private DateTime lastRecord;
        private bool firstRecord;

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

        /*public ModbusLogger(string connName, string databaseName, Dictionary<Type, string> collectionNames) {
            this._dataService = new MonitorDataService(connName, databaseName, collectionNames);
            this._alertService = new AlertService(connName, databaseName, collectionNames[typeof(ActionItem)], collectionNames[typeof(MonitorAlert)]);
            this.loggingEnabled = false;
            this._modbusService = new ModbusService();
        }*/

        public async Task Read() {
            var result = await this._modbusService.Read(this._device.IpAddress, this._device.Port, 
                this._device.ModbusConfiguration);
            if (result._success) {
                var now = DateTime.Now;
                this._alerts = new List<AlertRecord>();
                if (result.HoldingRegisters != null) {
                    /*var analogRaw = new ArraySegment<ushort>(result.HoldingRegisters, 
                        this._channelMapping.AnalogStart, 
                        (this._channelMapping.AnalogStop - this._channelMapping.AnalogStart) + 1)
                        .ToArray();*/
                    var analogRaw = new ArraySegment<ushort>(result.HoldingRegisters, this._device.ChannelMapping.AnalogStart, 
                        (this._device.ChannelMapping.AnalogStop - this._device.ChannelMapping.AnalogStart) + 1).ToArray();
                    await this.ProcessAnalogReadings(analogRaw, now);
                    MonitorData monitorData = new MonitorData();
                    monitorData.TimeStamp = now;
                    monitorData.analogData = this._alerts
                        .Select(e => new ItemStatus() { 
                            Item=e.DisplayName,
                            State=e.CurrentState.ToString(),
                            Value=e.ChannelReading.ToString()
                        
                        }).ToList();
                    await this._alertService.ProcessAlerts(this._alerts,now);
                }
            } else {
                this.LogError("Modbus read failed");
            }
        }

        private async Task ProcessAnalogReadings(ushort[] raw, DateTime now) {
            List<AnalogReading> readings = new List<AnalogReading>();
            foreach(var aItem in this._dataService.AnalogItems) {
                var reading = new AnalogReading();
                reading.itemid = aItem._id;
                if (aItem.reglen == 2) {
                    reading.value = Converters.ToInt32(raw[aItem.reg], raw[aItem.reg + 1])/aItem.factor;
                } else {
                    reading.value = raw[aItem.reg]/aItem.factor;
                }             
                readings.Add(reading);
                var alert=this._dataService.MonitorAlerts.FirstOrDefault(e => e.channelId == aItem._id);
                if (alert != null) {
                    ActionType state=ActionType.Okay;
                    if ((int)reading.value <= aItem.l3setpoint) {
                        state = aItem.l3action;
                    } else if ((int)reading.value <= aItem.l2setpoint) {
                        state = aItem.l2action;
                    } else if ((int)reading.value <= aItem.l1setpoint) {
                        state = aItem.l1action;
                    }
                    this._alerts.Add(new AlertRecord(alert,(float)reading.value,state));
                } else {
                    this.LogError($"AnalogChannel: {aItem.identifier} alert not found");
                } 
            }
            if (this.CheckSave(now, this.lastRecord)) {
                this.lastRecord = now;
                await this._dataService.InsertOneAsync(new AnalogReadings() {
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
            await this._alertService.Initialize();
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
