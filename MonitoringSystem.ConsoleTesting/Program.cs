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
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using MonitoringData.Infrastructure.Services.DataAccess;

namespace MonitoringSystem.ConsoleTesting {


    public class Program {
        static async Task Main(string[] args) {
            //await CreateConfigDatabse();
            //await CreateReadingsDatabase();
            //await TestReading();
            //var controller = new DeviceController();
            //await controller.Load();
            //await controller.Start();
            //controller.Run();



            //using var context = new FacilityContext();
            //var analogAlerts = await context.Alerts.Include(e => e.InputChannel).ToListAsync();
            //foreach(var alert in analogAlerts) {
            //    alert.DisplayName = alert.InputChannel.DisplayName;
            //}
            //context.UpdateRange(analogAlerts);
            //var ret = await context.SaveChangesAsync();
            //if (ret > 0) {
            //    Console.WriteLine("Alerts should be saved");
            //} else {
            //    Console.WriteLine("Error: Failed to save alerts");
            //}
            //Console.WriteLine("Press any key to exit");
            //Console.ReadKey();

            //var client = new MongoClient("mongodb://172.20.3.30");
            //var database = client.GetDatabase("epi2_data");
            //var alertItems = database.GetCollection<MonitorAlert>("alert_items");

            //await AlertItemUpdateEnabled();

            await RunDataLogger();
            //await TestAlerts();
            //ActionItemUpdate();
        }

        public static async Task RunDataLogger() {
            var datalogger = new DataLoggerWrapper();
            await datalogger.StartAsync();
            Console.WriteLine("Press q to exit");
            do {

            } while (Console.ReadKey().Key != ConsoleKey.Q);

            Console.WriteLine("Exiting program");
        }

        //public static async Task TestAlerts() {
        //    var client = new MongoClient("mongodb://172.20.3.30");
        //    var database = client.GetDatabase("epi2_data");
        //    IDataLogger dataLogger= new ModbusDataLogger(new MonitorDataService("mongodb://172.20.3.30", "epi2_data"), new FacilityContext());
        //    await dataLogger.Load();
        //    ConsoleKey key;
        //    do {
        //        await dataLogger.Read();
        //        Console.WriteLine("Press Enter to continue, Q to quit");
        //        key = Console.ReadKey().Key;
        //    } while (key != ConsoleKey.Q);

        //}
        static async Task ClearAlert(AlertReading reading,MonitorAlert alert, IMongoCollection<MonitorAlert> alerts) {
            var update = Builders<MonitorAlert>.Update
                .Set(e => e.CurrentState, reading.value)
                .Set(e => e.latched, false);
            await alerts.UpdateOneAsync(e => e._id == alert._id, update);
        }

        static UpdateDefinition<MonitorAlert> AlertUpdateAlreadyLatch(AlertReading reading) {
            return Builders<MonitorAlert>.Update
                .Set(e => e.lastAlarm, reading.timestamp)
                .Set(e => e.CurrentState, reading.value);
        }

        static UpdateDefinition<MonitorAlert> AlertUpdateLatch(AlertReading reading) {
            return Builders<MonitorAlert>.Update
                .Set(e => e.lastAlarm, reading.timestamp)
                .Set(e => e.CurrentState, reading.value)
                .Set(e => e.latched, true);
        }

        public static async Task AlertUpdateName() {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("epi2_data");
            var alertItems = database.GetCollection<MonitorAlert>("alert_items");
            using var context = new FacilityContext();
            var alerts = await context.Alerts
                .Include(e => e.InputChannel)
                .ThenInclude(e => e.ModbusDevice)
                .Where(e => e.InputChannel.ModbusDevice.Identifier == "epi2")
                .ToListAsync();

            alerts.ForEach((alert) => {
                var update = Builders<MonitorAlert>.Update.Set(s => s.displayName, alert.DisplayName);
                var monitorAlert = alertItems.FindOneAndUpdate<MonitorAlert>(e => e._id == alert.Id, update);
                //monitorAlert.displayName = alert.DisplayName;
                //alertItems.UpdateOne(monitorAlert);
            });

            Console.WriteLine("Check database");
        }

        public static async Task AlertItemTypeUpdate() {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("epi2_data");
            var alertItems = database.GetCollection<MonitorAlert>("alert_items");
            using var context = new FacilityContext();
            var alerts = await context.Alerts
                .Include(e => e.InputChannel)
                .ThenInclude(e => e.ModbusDevice)
                .Where(e => e.InputChannel.ModbusDevice.Identifier == "epi2")
                .ToListAsync();

            alerts.ForEach((alert) => {
                var update = Builders<MonitorAlert>.Update.Set(s => s.itemType, alert.AlertItemType);
                var monitorAlert = alertItems.FindOneAndUpdate<MonitorAlert>(e => e._id == alert.Id, update);
                monitorAlert.displayName = alert.DisplayName;
                //alertItems.UpdateOne(monitorAlert);
            });

            Console.WriteLine("Check database");
        }

        public static async Task AlertItemUpdateEnabled() {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("epi2_data");
            var alertItems = database.GetCollection<MonitorAlert>("alert_items");
            using var context = new FacilityContext();
            var alerts = await context.Alerts
                .Include(e => e.InputChannel)
                .ThenInclude(e => e.ModbusDevice)
                .Where(e => e.InputChannel.ModbusDevice.Identifier == "epi2")
                .ToListAsync();

            alerts.ForEach((alert) => {
                var update = Builders<MonitorAlert>.Update.Set(s => s.enabled, alert.Enabled);
                var monitorAlert = alertItems.FindOneAndUpdate<MonitorAlert>(e => e._id == alert.Id, update);
                //monitorAlert.displayName = alert.DisplayName;
                //alertItems.UpdateOne(monitorAlert);
            });

            Console.WriteLine("Check database");
        }

        public static void ActionItemUpdate() {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("epi2_data");
            var actionItems = database.GetCollection<ActionItem>("action_items");
            Dictionary<int, ActionType> actionTypeMap = new Dictionary<int, ActionType>();
            actionTypeMap.Add(1, ActionType.Okay);
            actionTypeMap.Add(2, ActionType.Alarm);
            actionTypeMap.Add(3, ActionType.Warning);
            actionTypeMap.Add(4, ActionType.SoftWarn);
            actionTypeMap.Add(5, ActionType.Maintenance);
            actionTypeMap.Add(6, ActionType.Custom);
            foreach(var item in actionTypeMap) {
                var update = Builders<ActionItem>.Update.Set(s => s.actionType, item.Value);
                var actionItem = actionItems.FindOneAndUpdate(e => e._id == item.Key, update);
            }
            Console.WriteLine("Check Database");
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

    public class DataLoggerWrapper {
        private IDataLogger _dataLogger;
        private Timer _timer;
        
        public DataLoggerWrapper() {
            Dictionary<Type, string> collectionNames = new Dictionary<Type, string>();
            collectionNames.Add(typeof(MonitorAlert), "alert_items");
            collectionNames.Add(typeof(ActionItem), "action_items");
            collectionNames.Add(typeof(AnalogChannel), "analog_items");
            collectionNames.Add(typeof(DiscreteChannel), "discrete_items");
            collectionNames.Add(typeof(VirtualChannel), "virtual_items");
            collectionNames.Add(typeof(OutputItem), "output_items");

            collectionNames.Add(typeof(AnalogReading), "analog_readings");
            collectionNames.Add(typeof(DiscreteReading), "discrete_readings");
            collectionNames.Add(typeof(VirtualReading), "virtual_readings");
            collectionNames.Add(typeof(OutputReading), "output_readings");
            collectionNames.Add(typeof(AlertReading), "alert_readings");
            collectionNames.Add(typeof(ActionReading), "action_readings");

            collectionNames.Add(typeof(DeviceReading), "device_readings");
            this._dataLogger = new ModbusDataLogger("mongodb://172.20.3.30", "epi2_data","","",collectionNames, new FacilityContext());
        }
        public async Task StartAsync() {
            Console.WriteLine("Starting Logging Service");
            await this._dataLogger.Load();
            this._timer = new Timer(this.DataLogCallback, null,TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }
        public async void DataLogCallback(object state) {
            await this._dataLogger.Read();
            Console.WriteLine($"{DateTime.Now}: Logged Data");
        }
    }
}
