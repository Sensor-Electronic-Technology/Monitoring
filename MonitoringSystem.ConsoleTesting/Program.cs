using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringData.Infrastructure.Model;
using MonitoringSystem.Shared.Data;
using MonitoringData.Infrastructure.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Timers;
using System.Text;
using System.IO;

namespace MonitoringSystem.ConsoleTesting {
    public class Program {
        static async Task Main(string[] args) {
            //await CreateConfigDatabse();
            //await CreateReadingsDatabase();
            //await TestReading();
            var controller = new DeviceController();
            await controller.Load();
            await controller.Start();
            controller.Run();
        }

        static async Task WriteOutFile() {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("Epi1_data_testing");

            var analogItems = database.GetCollection<AnalogChannel>("analog_items").Find(_ => true).ToList();
            var analogReadings = database.GetCollection<AnalogReading>("analog_readings");

            var start = new DateTime(2022, 3, 16, 11, 50, 0);
            var stop = new DateTime(2022, 3, 16, 12, 0, 0);

            Console.WriteLine("Starting query");
            var aReadings = analogReadings.Find(_ => true).ToList();
            var headers = aReadings.Select(e => e.itemid).OrderBy(e => e).Distinct().ToList();
            StringBuilder builder = new StringBuilder();
            headers.ForEach((id) => {
                builder.Append($"{id}\t");
            });
            var groupedReadings = aReadings.GroupBy(e => e.timestamp).ToList();
            Console.WriteLine($"Query Completed.  Count: {aReadings.Count()}");
            List<string> lines = new List<string>();

            groupedReadings.ForEach((item) => {
                StringBuilder builder = new StringBuilder();
                builder.Append($"{item.Key.Subtract(new TimeSpan(5, 0, 0))}\t");
                item.OrderBy(e => e.itemid).Select(e => $"{e.value / 10}\t").ToList().ForEach(s => builder.Append(s));
                lines.Add(builder.ToString());
            });
            Console.WriteLine("Writing Out Data");
            File.WriteAllLines(@"C:\MonitorFiles\epi1_newDataTest.txt", lines);
            Console.WriteLine("Check File");
        }

        static async Task CreateConfigDatabse() {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.30");
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .Select(e => new DeviceDTO() { Identifier = e.Identifier, NetworkConfiguration = e.NetworkConfiguration })
                .FirstOrDefault(e => e.Identifier == "epi2");

            if (device != null) {
                Console.WriteLine($"Device {device.Identifier.ToLower()} found");
                var database = client.GetDatabase($"{device.Identifier.ToLower()}_data");

                Console.WriteLine("Creating Collections");
                await database.CreateCollectionAsync("analog_items");
                await database.CreateCollectionAsync("discrete_items");
                await database.CreateCollectionAsync("output_items");
                await database.CreateCollectionAsync("virtual_items");
                await database.CreateCollectionAsync("action_items");
                await database.CreateCollectionAsync("alert_items");
                await database.CreateCollectionAsync("device_items");

                Console.WriteLine("Creating Analog Channels");

                IMongoCollection<AnalogChannel> analogItems = database.GetCollection<AnalogChannel>("analog_items");
                var analogChannels = await context.Channels.OfType<AnalogInput>()
                    .AsNoTracking()
                    .Include(e=>e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e=>e.SystemChannel)
                    .Select(e=>new AnalogChannel() { identifier=e.DisplayName,_id=e.Id})
                    .ToListAsync();

                var discreteItems = database.GetCollection<DiscreteChannel>("discrete_items");
                var discreteChannels = await context.Channels.OfType<DiscreteInput>()
                    .AsNoTracking()
                    .Include(e=>e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e=>e.SystemChannel)
                    .Select(e => new DiscreteChannel() { identifier = e.DisplayName, _id = e.Id })
                    .ToListAsync();

                IMongoCollection<OutputItem> outputItems = database.GetCollection<OutputItem>("output_items");
                var outputs = await context.Channels.OfType<OutputChannel>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e=>e.SystemChannel)
                    .Select(e => new OutputItem() { identifier = e.DisplayName, _id = e.Id })
                    .ToListAsync();

                IMongoCollection<ActionItem> actionItems = database.GetCollection<ActionItem>("action_items");
                var actions = await context.FacilityActions
                    .AsNoTracking()
                    .Select(e => new ActionItem() { 
                        identifier = e.ActionName, 
                        _id = e.Id,
                        EmailEnabled=e.EmailEnabled,
                        EmailPeriod=e.EmailPeriod})
                    .ToListAsync();

                IMongoCollection<VirtualChannel> virtualItems = database.GetCollection<VirtualChannel>("virtual_items");
                var virtualChannels = await context.Channels.OfType<VirtualInput>()
                    .AsNoTracking()
                    .Include(e=>e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new VirtualChannel() { identifier = e.DisplayName, _id = e.Id })
                    .ToListAsync();

                IMongoCollection<MonitorAlert> monitorAlerts = database.GetCollection<MonitorAlert>("alert_items");
                var alerts = await context.Channels.OfType<InputChannel>()
                    .AsNoTracking()
                    .Include(e => e.Alert)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .Select(e => e.Alert)
                    .Select(e => new MonitorAlert() { 
                        _id = e.Id, 
                        channelId = e.InputChannel.Id,
                        enabled = e.Enabled,
                        bypassed = e.Bypass,
                        bypassResetTime = e.BypassResetTime,
                        latched = false,
                        CurrentState = ActionType.Okay })
                    .ToListAsync();

                await analogItems.InsertManyAsync(analogChannels);
                await discreteItems.InsertManyAsync(discreteChannels);
                await outputItems.InsertManyAsync(outputs);
                await actionItems.InsertManyAsync(actions);
                await monitorAlerts.InsertManyAsync(alerts);
                await virtualItems.InsertManyAsync(virtualChannels);
                

                Console.WriteLine("Check database");


            } else {
                Console.WriteLine("Device Not Found, Please Check Identifier");
            }
            Console.ReadKey();
        }
        static async Task CreateReadingsDatabase() {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.30");
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .Select(e => new DeviceDTO() { Identifier = e.Identifier, NetworkConfiguration = e.NetworkConfiguration })
                .FirstOrDefault(e => e.Identifier == "epi2");

            if (device != null) {
                Console.WriteLine($"Device {device.Identifier} found");
                var database = client.GetDatabase($"{device.Identifier.ToLower()}_data");
                Console.WriteLine("Creating Collections");

                await database.CreateCollectionAsync("analog_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("discrete_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("output_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("virtual_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("action_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("alert_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("device_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                Console.WriteLine("Gettting Collections");
                var analogReadings = database.GetCollection<AnalogReading>("analog_readings");
                var discreteReadings = database.GetCollection<DiscreteReading>("discrete_readings");
                var outputReadings = database.GetCollection<OutputReading>("output_readings");
                var virtualReadings = database.GetCollection<VirtualReading>("virtual_readings");
                var actionReadings = database.GetCollection<ActionReading>("action_readings");
                var alertReadings = database.GetCollection<AlertReading>("alert_readings");
                var deviceReadings = database.GetCollection<DeviceReading>("device_readings");

                Console.WriteLine("Creating TimeSeries Indices");

                analogReadings.Indexes.CreateOne(new CreateIndexModel<AnalogReading>(Builders<AnalogReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                discreteReadings.Indexes.CreateOne(new CreateIndexModel<DiscreteReading>(Builders<DiscreteReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                outputReadings.Indexes.CreateOne(new CreateIndexModel<OutputReading>(Builders<OutputReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                virtualReadings.Indexes.CreateOne(new CreateIndexModel<VirtualReading>(Builders<VirtualReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                alertReadings.Indexes.CreateOne(new CreateIndexModel<AlertReading>(Builders<AlertReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                actionReadings.Indexes.CreateOne(new CreateIndexModel<ActionReading>(Builders<ActionReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                deviceReadings.Indexes.CreateOne(new CreateIndexModel<DeviceReading>(Builders<DeviceReading>.IndexKeys.Ascending(x => x.itemid),
                new CreateIndexOptions()),
                new CreateOneIndexOptions());


                Console.WriteLine("Inserting Initialization Data");
                await analogReadings.InsertOneAsync(new AnalogReading() { itemid = -1, timestamp = DateTime.Now, value = 0 });
                await discreteReadings.InsertOneAsync(new DiscreteReading() { itemid = -1, timestamp = DateTime.Now, value = false });
                await outputReadings.InsertOneAsync(new OutputReading() { itemid = -1, timestamp = DateTime.Now, value = false });
                await virtualReadings.InsertOneAsync(new VirtualReading() { itemid = -1, timestamp = DateTime.Now, value = false });
                await alertReadings.InsertOneAsync(new AlertReading() { itemid = -1, timestamp = DateTime.Now, value = ActionType.Custom });
                await actionReadings.InsertOneAsync(new ActionReading() { itemid = -1, timestamp = DateTime.Now, value = false });
                await deviceReadings.InsertOneAsync(new DeviceReading() { itemid = -1, timestamp = DateTime.Now, value = DeviceState.OKAY });

                Console.WriteLine("Should be done, check database");
            } else {
                Console.WriteLine("Error: Device not found");
            }

        }
    
        static async Task TestReading() {
            using var context = new FacilityContext();
            var device = await context.Devices.AsNoTracking()
                .OfType<ModbusDevice>()
                .FirstOrDefaultAsync(e => e.Identifier == "epi2");
            if (device != null) {
                var client = new MongoClient("mongodb://172.20.3.30");
                var database = client.GetDatabase("epi2_data");

                Console.WriteLine("Device found, setting up read");
                var networkConfig = device.NetworkConfiguration;
                var modbusConfig = networkConfig.ModbusConfig;
                var channelMapping = modbusConfig.ChannelMapping;

                var result = await ModbusService.Read(networkConfig.IPAddress, networkConfig.Port, networkConfig.ModbusConfig);
                var discreteInputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.DiscreteStart, (channelMapping.DiscreteStop - channelMapping.DiscreteStart) + 1).ToArray();
                var outputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.OutputStart, (channelMapping.OutputStop - channelMapping.OutputStart) + 1).ToArray();
                var actions = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.ActionStart, (channelMapping.ActionStop - channelMapping.ActionStart) + 1).ToArray();
                var analogInputs = new ArraySegment<ushort>(result.InputRegisters, channelMapping.AnalogStart, (channelMapping.AnalogStop - channelMapping.AnalogStart) + 1).ToArray();
                var alerts = new ArraySegment<ushort>(result.HoldingRegisters, channelMapping.AlertStart, (channelMapping.AlertStop - channelMapping.AlertStart) + 1).ToArray();
                var virts = new ArraySegment<bool>(result.Coils, channelMapping.VirtualStart, (channelMapping.VirtualStop - channelMapping.VirtualStart) + 1).ToArray();

                //Get Configurations and readings collections
                var analogItems = database.GetCollection<AnalogChannel>("analog_items");
                var discreteItems = database.GetCollection<DiscreteChannel>("discrete_items");
                var outputItems = database.GetCollection<OutputItem>("output_items");
                var virtualItems = database.GetCollection<VirtualChannel>("virtual_items");
                var actionItems = database.GetCollection<ActionItem>("action_items");
                var alertItems = database.GetCollection<MonitorAlert>("alert_items");

                var analogReadings = database.GetCollection<AnalogReading>("analog_readings");
                var discreteReadings = database.GetCollection<DiscreteReading>("discrete_readings");
                var outputReadings = database.GetCollection<OutputReading>("analog_readings");
                var virtualReadings = database.GetCollection<VirtualReading>("analog_readings");
                var actionReadings = database.GetCollection<ActionReading>("analog_readings");
                var alertReadings = database.GetCollection<AlertReading>("analog_readings");

                var analogConfig = await analogItems.Find(_=>true).ToListAsync();
                var discreteConfig = await analogItems.Find(_=> true).ToListAsync();
                var outputConfig = await analogItems.Find(_=> true).ToListAsync();
                var virtualConfig = await analogItems.Find(_=> true).ToListAsync();
                var actionConfig = await analogItems.Find(_=> true).ToListAsync();
                var alertConfig = await alertItems.Find(_=> true).ToListAsync();

                var now = DateTime.Now;

                List<AnalogReading> aTempReadings = new List<AnalogReading>();
                for(int i = 0; i < analogInputs.Length; i++) {
                    aTempReadings.Add(new AnalogReading() { itemid = analogConfig[i]._id, timestamp = now, value = analogInputs[i] });
                }
                await analogReadings.InsertManyAsync(aTempReadings);
                Console.WriteLine("Done: Check Analog Readings");

            } else {
                Console.WriteLine("Error: Device not found");
            }
        }
    }

    public class DeviceController {
        private Timer timer;
        private FacilityContext _context;
        private IMongoCollection<AnalogChannel> _analogChannels;
        private IMongoCollection<DiscreteChannel> _discreteChannels;
        private IMongoCollection<VirtualChannel> _virtualChannels;
        private IMongoCollection<OutputItem> _outputChannels;
        private IMongoCollection<ActionItem> _actionItems;
        private IMongoCollection<MonitorAlert> _monitorAlerts;

        private IMongoCollection<AnalogReading> _analogReadings;
        private IMongoCollection<DiscreteReading> _discreteReadings;
        private IMongoCollection<VirtualReading> _virtualReadings;
        private IMongoCollection<OutputReading> _outputReadings;
        private IMongoCollection<ActionReading> _actionReadings;
        private IMongoCollection<AlertReading> _alertReadings;

        private List<AnalogChannel> _analogConfig;
        private List<OutputItem> _outputConfig;
        private List<DiscreteChannel> _discreteConfig;
        private List<VirtualChannel> _virtualConfig;
        private List<ActionItem> _actionConfig;
        private List<MonitorAlert> _alertConfig;

        private NetworkConfiguration _networkConfig;
        private ModbusConfig _modbusConfig;
        private ChannelRegisterMapping _channelMapping;

        private bool loaded=false;

        public DeviceController() {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("epi2_data");
            this._context = new FacilityContext();
            this._analogChannels = database.GetCollection<AnalogChannel>("analog_items");
            this._discreteChannels = database.GetCollection<DiscreteChannel>("discrete_items");
            this._outputChannels = database.GetCollection<OutputItem>("output_items");
            this._virtualChannels = database.GetCollection<VirtualChannel>("virtual_items");
            this._actionItems = database.GetCollection<ActionItem>("action_items");
            this._monitorAlerts = database.GetCollection<MonitorAlert>("alert_items");

            this._analogReadings = database.GetCollection<AnalogReading>("analog_readings");
            this._discreteReadings = database.GetCollection<DiscreteReading>("discrete_readings");
            this._outputReadings = database.GetCollection<OutputReading>("output_readings");
            this._virtualReadings = database.GetCollection<VirtualReading>("virtual_readings");
            this._alertReadings = database.GetCollection<AlertReading>("alert_readings");
            this._actionReadings = database.GetCollection<ActionReading>("action_readings");         
        }

        public Task Start() {
            this.timer = new Timer();
            this.timer.Enabled = true;
            this.timer.AutoReset = true;
            this.timer.Interval = 1000;
            this.timer.Elapsed += Timer_Elapsed;
            return Task.CompletedTask;
        }

        public void Run() {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            var result=ModbusService.Read(this._networkConfig.IPAddress,this._networkConfig.Port,this._modbusConfig).GetAwaiter().GetResult();

            var discreteInputs = new ArraySegment<bool>(result.DiscreteInputs, this._channelMapping.DiscreteStart, (this._channelMapping.DiscreteStop - this._channelMapping.DiscreteStart) + 1).ToArray();
            var outputs = new ArraySegment<bool>(result.DiscreteInputs, this._channelMapping.OutputStart, (this._channelMapping.OutputStop - this._channelMapping.OutputStart) + 1).ToArray();
            var actions = new ArraySegment<bool>(result.DiscreteInputs, this._channelMapping.ActionStart, (this._channelMapping.ActionStop - this._channelMapping.ActionStart) + 1).ToArray();
            var analogInputs = new ArraySegment<ushort>(result.InputRegisters, this._channelMapping.AnalogStart, (this._channelMapping.AnalogStop - this._channelMapping.AnalogStart) + 1).ToArray();
            var alerts = new ArraySegment<ushort>(result.HoldingRegisters, this._channelMapping.AlertStart, (this._channelMapping.AlertStop - this._channelMapping.AlertStart) + 1).ToArray();
            var virts = new ArraySegment<bool>(result.Coils, this._channelMapping.VirtualStart, (this._channelMapping.VirtualStop - this._channelMapping.VirtualStart) + 1).ToArray();

            var now = DateTime.Now;

            List<AnalogReading> aTempReadings = new List<AnalogReading>();
            for (int i = 0; i < analogInputs.Length; i++) {
                aTempReadings.Add(new AnalogReading() { itemid = this._analogConfig[i]._id, timestamp = now, value = analogInputs[i] });
            }
            this._analogReadings.InsertMany(aTempReadings);

            List<DiscreteReading> dTempReadings = new List<DiscreteReading>();
            for (int i = 0; i < discreteInputs.Length; i++) {
                dTempReadings.Add(new DiscreteReading() { itemid = this._discreteConfig[i]._id, timestamp = now, value = discreteInputs[i] });
            }
            this._discreteReadings.InsertMany(dTempReadings);

            List<OutputReading> oReadings = new List<OutputReading>();
            for (int i = 0; i < outputs.Length; i++) {
                oReadings.Add(new OutputReading() { itemid = this._outputConfig[i]._id, timestamp = now, value = outputs[i] });
            }
            this._outputReadings.InsertMany(oReadings);

            List<ActionReading> actReadingTemp = new List<ActionReading>();
            for (int i = 0; i < actions.Length; i++) {
                actReadingTemp.Add(new ActionReading() { itemid = this._actionConfig[i]._id, timestamp = now, value = actions[i] });
            }
            this._actionReadings.InsertMany(actReadingTemp);

            List<AlertReading> alertReadingTemp = new List<AlertReading>();
            
            for(int i = 0; i < alerts.Length; i++) {
                var alertReading = new AlertReading() {
                    itemid = this._alertConfig[i]._id,
                    timestamp = now,
                    value = this.ToActionType(alerts[i])
                };

                switch (alertReading.value) {
                    case ActionType.Okay:
                        break;
                    case ActionType.Alarm:
                        Console.WriteLine($"Alarm: {this._alertConfig[i]._id} Value: {alertReading.value}");
                        break;
                    case ActionType.Warning:
                        Console.WriteLine($"Warn: {this._alertConfig[i]._id} Value: {alertReading.value}");
                        break;
                    case ActionType.SoftWarn:
                        Console.WriteLine($"SoftWarn: {this._alertConfig[i]._id} Value: {alertReading.value}");
                        break;
                    case ActionType.Maintenance:
                        Console.WriteLine($"Maint: {this._alertConfig[i]._id} Value: {alertReading.value}");
                        break;
                    case ActionType.Custom:
                        break;
                }
                alertReadingTemp.Add(alertReading);
            }
            this._alertReadings.InsertMany(alertReadingTemp);
        }

        public async Task Load() {
            this._analogConfig = await this._analogChannels.Find(_ => true).ToListAsync();
            this._discreteConfig = await this._discreteChannels.Find(_ => true).ToListAsync();
            this._outputConfig = await this._outputChannels.Find(_ => true).ToListAsync();
            this._virtualConfig = await this._virtualChannels.Find(_ => true).ToListAsync();
            this._alertConfig = await this._monitorAlerts.Find(_ => true).ToListAsync();
            this._actionConfig = await this._actionItems.Find(_ => true).ToListAsync();
            var device = await this._context.Devices.AsNoTracking()
                .OfType<ModbusDevice>()
                .FirstOrDefaultAsync(e => e.Identifier == "epi2");
            if(device!=null) {
                this.loaded = true;
                this._networkConfig = device.NetworkConfiguration;
                this._modbusConfig = this._networkConfig.ModbusConfig;
                this._channelMapping = this._modbusConfig.ChannelMapping;
                Console.WriteLine("Loading Completed");
            } else {
                Console.WriteLine("Error:Device not found");
                this.loaded = false;
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
    }
}
