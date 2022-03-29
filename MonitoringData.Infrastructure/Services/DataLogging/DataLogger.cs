using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringData.Infrastructure.Services.DataAccess;
using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Services {
    public interface IDataLogger {
        Task Read();
        Task Load();
    }

    public class ModbusDataLogger : IDataLogger {
        private readonly IMonitorDataRepo _dataService;
        private IAlertService _alertService;
        private readonly ILogger _logger;
        private readonly FacilityContext _context;
        private string deviceIdentifier;

        private NetworkConfiguration _networkConfig;
        private ModbusConfig _modbusConfig;
        private ChannelRegisterMapping _channelMapping;
        private bool initialized = false;
        private bool loggingEnabled = false;
        private IList<ItemAlert> _itemAlerts;

        public ModbusDataLogger(IMonitorDataRepo dataService,ILogger<ModbusDataLogger> logger,IAlertService alertService,FacilityContext context) {
            this._dataService = dataService;
            this._alertService = alertService;
            this._logger = logger;
            this._context = context;
            this.loggingEnabled = true;
        }

        public ModbusDataLogger(string connName,string databaseName,Dictionary<Type,string> collectionNames) {
            this._dataService = new MonitorDataService(connName,databaseName,collectionNames);
            this._context = new FacilityContext();            
            this._alertService = new AlertService(connName,databaseName,collectionNames[typeof(ActionItem)],collectionNames[typeof(MonitorAlert)]);
            this.loggingEnabled = true;
        }
        public async Task Read() {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var result = await ModbusService.Read(this._networkConfig.IPAddress, this._networkConfig.Port, this._modbusConfig);
            if (result._success) {
                var discreteRaw = new ArraySegment<bool>(result.DiscreteInputs, this._channelMapping.DiscreteStart, (this._channelMapping.DiscreteStop - this._channelMapping.DiscreteStart) + 1).ToArray();
                var outputsRaw = new ArraySegment<bool>(result.DiscreteInputs, this._channelMapping.OutputStart, (this._channelMapping.OutputStop - this._channelMapping.OutputStart) + 1).ToArray();
                var actionsRaw = new ArraySegment<bool>(result.DiscreteInputs, this._channelMapping.ActionStart, (this._channelMapping.ActionStop - this._channelMapping.ActionStart) + 1).ToArray();
                var analogRaw = new ArraySegment<ushort>(result.InputRegisters, this._channelMapping.AnalogStart, (this._channelMapping.AnalogStop - this._channelMapping.AnalogStart) + 1).ToArray();
                var alertsRaw = new ArraySegment<ushort>(result.HoldingRegisters, this._channelMapping.AlertStart, (this._channelMapping.AlertStop - this._channelMapping.AlertStart) + 1).ToArray();
                var virtualRaw = new ArraySegment<bool>(result.Coils, this._channelMapping.VirtualStart, (this._channelMapping.VirtualStop - this._channelMapping.VirtualStart) + 1).ToArray();
                var now = DateTime.Now;
                //this._itemAlerts = new List<ItemAlert>();
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

                var t1= this.ProcessAlertReadings(alertsRaw, now,this._dataService.MonitorAlerts.AsReadOnly());
                var t2=this.ProcessAnalogReadings(analogRaw, now, this._dataService.AnalogItems.AsReadOnly());
                var t3=this.ProcessDiscreteReadings(discreteRaw, now, this._dataService.DiscreteItems.AsReadOnly());
                var t4=this.ProcessVirtualReadings(virtualRaw, now, this._dataService.VirtualItems.AsReadOnly());
                var t5=this.ProcessOutputReadings(outputsRaw, now, this._dataService.OutputItems.AsReadOnly());
                var t6=this.ProcessActionReadings(actionsRaw, now, this._dataService.ActionItems.AsReadOnly());
                await Task.WhenAll(t1, t2, t3, t4, t5, t6);
                var alertReadings = t1.Result.readings;
                var analogAlerts = t1.Result.aAlerts;
                var discreteAlerts = t1.Result.dAlerts;
                var virtualAlerts = t1.Result.vAlerts;          
                var analogReadings = t2.Result;
                var discreteReading = t3.Result;
                var virtualReadings = t4.Result;

                var outputReadings = t5.Result;
                var actionReadings = t6.Result;



                //var t7=this._alertService.ProcessAlerts(this._itemAlerts);
            } else {
                this._logger.LogError("Modbus read failed");
            }
            Console.WriteLine($"Elapsed: {watch.ElapsedMilliseconds}");
            watch.Stop();
            watch.Restart();
        }

        public async Task Load() {
            await this._dataService.LoadAsync();
            await this._alertService.Initialize();
            //this._alertService = new AlertService(this._dataService);
            var device = await this._context.Devices.AsNoTracking()
                .OfType<ModbusDevice>()
                .FirstOrDefaultAsync(e => e.Identifier == "epi2");
            if (device != null) {
                this.initialized = true;
                this._networkConfig = device.NetworkConfiguration;
                this._modbusConfig = this._networkConfig.ModbusConfig;
                this._channelMapping = this._modbusConfig.ChannelMapping;
            } else {
                this.initialized = false;
            }
        }

        private Task<(List<AlertReading> readings, List<ItemAlert> aAlerts,List<ItemAlert> dAlerts,List<ItemAlert> vAlerts)> 
            ProcessAlertReadings(ushort[] raw,DateTime now,ReadOnlyCollection<MonitorAlert> monitorAlerts) {
            return Task<(List<AlertReading> readings, List<ItemAlert> aAlerts, List<ItemAlert> dAlerts, List<ItemAlert> vAlerts)>.Factory.StartNew(() => {
                List<AlertReading> alertReadings = new List<AlertReading>();
                List<ItemAlert> itemAlerts = new List<ItemAlert>();
                for (int i = 0; i < raw.Length; i++) {
                    var alertReading = new AlertReading() {
                        itemid = monitorAlerts[i]._id,
                        timestamp = now,
                        value = this.ToActionType(raw[i])
                    };
                    alertReadings.Add(alertReading);
                    itemAlerts.Add(new ItemAlert(monitorAlerts[i], alertReading.value));
                }
                var aAlerts = itemAlerts.Where(e => e.Alert.itemType == AlertItemType.Analog).ToList();
                var dAlerts = itemAlerts.Where(e => e.Alert.itemType == AlertItemType.Discrete).ToList();
                var vAlerts = itemAlerts.Where(e => e.Alert.itemType == AlertItemType.Virtual).ToList();
                return (alertReadings,aAlerts,dAlerts,vAlerts);
            });
        }

        private Task<List<AnalogReading>> ProcessAnalogReadings(ushort[] raw, DateTime now, ReadOnlyCollection<AnalogChannel> analogItems) {
            return Task<List<AnalogReading>>.Factory.StartNew(() => {
                List<AnalogReading> analogReadings = new List<AnalogReading>();
                if (analogItems.Count == raw.Length) {
                    for (int i = 0; i < raw.Length; i++) {
                        var analogReading = new AnalogReading() { itemid = analogItems[i]._id, timestamp = now, value = raw[i] / 10 };
                        analogReadings.Add(analogReading);
                        //var itemAlert = this._itemAlerts.FirstOrDefault(e => e.Alert.channelId == analogReading.itemid);
                        //if (itemAlert != null) {
                        //    itemAlert.Reading = (float)analogReading.value;
                        //} else {
                        //    this.LogError("Analog ItemAlert not found");
                        //}
                    }
                    //await this._dataService.InsertManyAsync(analogReadings);
                } else {
                    this.LogError("Error: AnalogItems count doesn't match raw data count");
                }
                return analogReadings;
            });

        }

        private Task<List<DiscreteReading>> ProcessDiscreteReadings(bool[] raw,DateTime now, ReadOnlyCollection<DiscreteChannel> discreteItems) {
            return Task<List<DiscreteReading>>.Factory.StartNew(() => {
                List<DiscreteReading> readings = new List<DiscreteReading>();
                if (discreteItems.Count == raw.Length) {
                    for (int i = 0; i < raw.Length; i++) {
                        var reading = new DiscreteReading() {
                            itemid = discreteItems[i]._id,
                            timestamp = now,
                            value = raw[i]
                        };
                        readings.Add(reading);
                        //var itemAlert = this._itemAlerts.FirstOrDefault(e => e.Alert.channelId == reading.itemid);
                        //if (itemAlert != null) {
                        //    itemAlert.Reading = reading.value == true ? 1.00f : 0.00f;
                        //} else {
                        //    this.LogError("Discrete ItemAlert not found");
                        //}
                    }
                    //await this._dataService.InsertManyAsync(readings);
                } else {
                    this.LogError("Error: DiscreteItems count doesn't match raw data count");
                }
                return readings;
            });
        }

        private Task<List<VirtualReading>> ProcessVirtualReadings(bool[] raw, DateTime now,ReadOnlyCollection<VirtualChannel> virtualItems) {
            return Task<List<VirtualReading>>.Factory.StartNew(() => {
                List<VirtualReading> readings = new List<VirtualReading>();
                if (virtualItems.Count == raw.Length) {
                    for (int i = 0; i < raw.Length; i++) {
                        var reading = new VirtualReading() {
                            itemid = virtualItems[i]._id,
                            timestamp = now,
                            value = raw[i]
                        };
                        readings.Add(reading);
                        //var itemAlert = this._itemAlerts.FirstOrDefault(e => e.Alert.channelId == reading.itemid);
                        //if (itemAlert != null) {
                        //    itemAlert.Reading = reading.value == true ? 1.00f : 0.00f;
                        //} else {
                        //    this.LogError("Virual ItemAlert not found");
                        //}
                    }
                    //await this._dataService.InsertManyAsync(readings);
                } else {
                    this.LogError("Error: Virtualitems count doesn't match raw data count");
                }
                return readings;
            });
        }

        private Task<List<OutputReading>> ProcessOutputReadings(bool[] raw,DateTime now,ReadOnlyCollection<OutputItem> outputItems) {
            return Task<List<OutputReading>>.Factory.StartNew(() => {
                List<OutputReading> readings = new List<OutputReading>();
                if (outputItems.Count == raw.Length) {
                    for (int i = 0; i < raw.Length; i++) {
                        readings.Add(new OutputReading() { itemid = outputItems[i]._id, timestamp = now, value = raw[i] });
                    }
                    //await this._dataService.InsertManyAsync(readings);
                } else {
                    this.LogError("Error: OutputItems count doesn't match raw data count");
                }
                return readings;
            });
        }

        private Task<List<ActionReading>> ProcessActionReadings(bool[] raw,DateTime now,ReadOnlyCollection<ActionItem> actionItems) {
            return Task<List<ActionReading>>.Factory.StartNew(() => {
                List<ActionReading> readings = new List<ActionReading>();
                if (actionItems.Count == raw.Length) {
                    for (int i = 0; i < raw.Length; i++) {
                        readings.Add(new ActionReading() {
                            itemid = actionItems[i]._id,
                            timestamp = now,
                            value = raw[i]
                        });
                    }
                    //await this._dataService.InsertManyAsync(readings);
                } else {
                    this.LogError("Error: ActionItem Count doesn't match raw data count");
                }
                return readings;
            });
        }

        private void LogInformation(string msg) {
            if (this.loggingEnabled) {
                this._logger.LogInformation(msg);
            }else {
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
    }

}
