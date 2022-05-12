﻿using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MonitoringData.Infrastructure.Events;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringData.Infrastructure.Services.DataAccess;
using MonitoringSystem.Shared.SignalR;
using MonitoringSystem.Shared.Data;
using MonitoringData.Infrastructure.Utilities;

namespace MonitoringData.Infrastructure.Services.DataLogging {
    public class ModbusLogger : IDataLogger, IConsumer<ReloadConsumer> {
        private readonly IMonitorDataRepo _dataService;
        private readonly IModbusService _modbusService;
        private readonly IHubContext<MonitorHub, IMonitorHub> _monitorHub;
        private IAlertService _alertService;
        private readonly ILogger _logger;

        private bool loggingEnabled = false;
        private NetworkConfiguration _networkConfig;
        private ModbusConfig _modbusConfig;
        private ChannelRegisterMapping _channelMapping;
        private IList<AlertRecord> _alerts;

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
        }

        public ModbusLogger(string connName, string databaseName, Dictionary<Type, string> collectionNames) {
            this._dataService = new MonitorDataService(connName, databaseName, collectionNames);
            this._alertService = new AlertService(connName, databaseName, collectionNames[typeof(ActionItem)], collectionNames[typeof(MonitorAlert)]);
            this.loggingEnabled = false;
            this._modbusService = new ModbusService();
        }

        public async Task Read() {
            var result = await this._modbusService.Read(this._networkConfig.IPAddress, this._networkConfig.Port, this._modbusConfig);
            if (result._success) {
                var now = DateTime.Now;
                this._alerts = new List<AlertRecord>();
                if (result.HoldingRegisters != null) {
                    var analogRaw = new ArraySegment<ushort>(result.HoldingRegisters, 
                        this._channelMapping.AnalogStart, 
                        (this._channelMapping.AnalogStop - this._channelMapping.AnalogStart) + 1)
                        .ToArray();
                    await this.ProcessAnalogReadings(analogRaw, now);
                    MonitorData monitorData = new MonitorData();
                    monitorData.TimeStamp = now;
                    monitorData.analogData = this._alerts
                        .Where(e => e.Enabled && e.ItemType == AlertItemType.Analog)
                        .Select(e => new ItemStatus() { 
                            Item=e.DisplayName,
                            State=e.CurrentState.ToString(),
                            Value=e.ChannelReading.ToString()
                        
                        }).ToList();
                    await this._alertService.ProcessAlerts(this._alerts,now);
                    await this._monitorHub.Clients.All.ShowCurrent(monitorData);
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
                    ActionType state;
                    if (reading.value <= aItem.l3setpoint) {
                        state = aItem.l3action;
                    } else if (reading.value <= aItem.l2setpoint) {
                        state = aItem.l2action;
                    } else if (reading.value <= aItem.l3setpoint) {
                        state = aItem.l1action;
                    } else {
                        state = ActionType.Okay;
                    }
                    this._alerts.Add(new AlertRecord(alert,(float)reading.value,state));
                } else {
                    this.LogError($"AnalogChannel: {aItem.identifier} alert not found");
                } 
            }
            await this._dataService.InsertOneAsync(new AnalogReadings() {readings=readings.ToArray(),timestamp=now});
        }

        public async Task Load() {
            await this._dataService.LoadAsync();
            await this._alertService.Initialize();
            this._networkConfig = this._dataService.MonitorDevice.NetworkConfiguration;
            this._modbusConfig = this._networkConfig.ModbusConfig;
            this._channelMapping = this._modbusConfig.ChannelMapping;
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
