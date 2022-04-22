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
using MonitoringData.Infrastructure.Services.AlertServices;

namespace MonitoringSystem.ConsoleTesting {
    public record class Test {
        public int Value { get; set; }
    }


    public class Program {
        static readonly CancellationTokenSource s_cts = new CancellationTokenSource();
        static async Task Main(string[] args) {
            //await CreateMongoDevice("epi1");
            //Console.WriteLine("Epi updated");
            //await CreateMongoDevice("epi2");
            //Console.WriteLine("Epi2 Updated");
            //Console.WriteLine("Check Databases");
            //ActionItemUpdate();
            //Console.WriteLine("Press any key to continue");
            //Console.ReadKey();
            //UpdateActionEmailSettings();
            //await ModifyAnalog();
            //await WriteOutAlertFile();
            //await WriteOutAnalogFile();
            //await CreateConfigDatabase("epi1");
            //await CreateConfigDatabase("epi2");
            //await CreateReadingsDatabase("epi1");
            //await CreateReadingsDatabase("epi2");
            //await AlertUpdate("epi1");
            //await AlertUpdate("epi2");

            //await CreateConfigDatabase("epi2");
            //await CreateReadingsDatabase("epi2");
            //await RunDataLogger();
            //await TestAlerts();
            //await ModifyAnalog();
            //await UpdateChannels("epi1");
            //await UpdateChannels("epi2");


            //var client = new MongoClient("mongodb://172.20.3.30");
            //var database = client.GetDatabase("epi2_data");
            //var collection = database.GetCollection<AnalogChannel>("analog_items");
            //var items = (await collection.FindAsync(_ => true)).ToList();
            //foreach(var item in items) {
            //    Console.WriteLine($"A{item.identifier}: {item.factor}");
            //}
            //Console.ReadKey();

            //await WriteOutAnalogFile("epi1", new DateTime(2022, 4, 10, 0, 0, 0), new DateTime(2022, 4, 11, 0, 0, 0), @"C:\MonitorFiles\epi1_analogReadings_4-8_4-9.csv");
            await WriteOutAnalogFile("epi1", new DateTime(2022, 4, 11, 3, 0, 0), DateTime.Now, @"C:\MonitorFiles\epi2_analogReadings_4-20.csv");
            //var client = new MongoClient("mongodb://172.20.3.30");
            //var database = client.GetDatabase("epi1_data_test");

            //await CreateReadingsDatabaseNew("epi1");
            //await CreateReadingsDatabaseNew("epi2");

            //var client = new MongoClient("mongodb://172.20.3.41");
            //var database = client.GetDatabase("epi1_data");
            //var analogReadings = database.GetCollection<AnalogChannel>("analog_items");
            
            //using (var cursor = analogReadings.Watch()) {
            //    foreach(var change in cursor.ToEnumerable(s_cts.Token)) {
            //        Console.WriteLine(change.ToString());
                    
            //    }
            //}
            //Console.WriteLine("End of the line");
                //var next = cursor.Current.First();
            //Console.WriteLine(next.ToString());
        }

        static async Task WriteOutAnalogFile(string deviceName, DateTime start, DateTime stop, string fileName) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceName + "_data");

            var analogItems = database.GetCollection<AnalogChannel>("analog_items").Find(_ => true).ToList();
            var analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
            Console.WriteLine("Starting query");
            //var aReadings = await (await analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop)).ToListAsync();
            var aReadings = await (await analogReadings.FindAsync(_=>true)).ToListAsync();
            var headers = analogItems.Select(e => e.identifier).ToList();
            StringBuilder hbuilder = new StringBuilder();
            hbuilder.Append("timestamp,");
            headers.ForEach((id) => {
                hbuilder.Append($"{id},");
            });
            Console.WriteLine($"Query Completed.  Count: {aReadings.Count()}");
            List<string> lines = new List<string>();
            lines.Add(hbuilder.ToString());
            foreach(var readings in aReadings) {
                StringBuilder builder = new StringBuilder();
                builder.Append(readings.timestamp.ToString()+",");
                foreach(var reading in readings.readings) {
                    builder.Append($"{reading.value},");
                }
                lines.Add(builder.ToString());
            }
            Console.WriteLine("Writing Out Data");
            File.WriteAllLines(fileName, lines);
            Console.WriteLine("Check File");
        }

        public static async Task UpdateChannels(string deviceName) {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase(deviceName+"_data_temp");
            var analogItems = database.GetCollection<AnalogChannel>("analog_items");
            var discreteItems = database.GetCollection<DiscreteChannel>("discrete_items");
            var virtualItems = database.GetCollection<VirtualChannel>("virtual_items");

            var analogChannels = await context.Channels.OfType<AnalogInput>()
                .AsNoTracking()
                .Include(e => e.ModbusDevice)
                .Where(e => e.ModbusDevice.Identifier == deviceName)
                .ToListAsync();

            var discreteChannels = await context.Channels.OfType<DiscreteInput>()
                .AsNoTracking()
                .Include(e => e.ModbusDevice)
                .Where(e => e.ModbusDevice.Identifier == deviceName)
                .ToListAsync();

            var virtualChannels = await context.Channels.OfType<VirtualInput>()
                .AsNoTracking()
                .Include(e => e.ModbusDevice)
                .Where(e => e.ModbusDevice.Identifier == deviceName)
                .ToListAsync();

            foreach (var channel in analogChannels) {
                var update = Builders<AnalogChannel>.Update
                    .Set(e => e.factor, 10)
                    .Set(e=>e.display,channel.Connected)
                    .Set(e=>e.identifier,channel.DisplayName);
                await analogItems.UpdateOneAsync(e=>e._id==channel.Id,update);
            }

            foreach (var channel in discreteChannels) {
                var update = Builders<DiscreteChannel>.Update
                    .Set(e => e.display, channel.Connected)
                    .Set(e => e.identifier, channel.DisplayName);
                await discreteItems.UpdateOneAsync(e => e._id == channel.Id, update);
            }

            foreach (var channel in virtualChannels) {
                var update = Builders<VirtualChannel>.Update
                    .Set(e => e.display, channel.Connected)
                    .Set(e => e.identifier, channel.DisplayName);
                await virtualItems.UpdateOneAsync(e => e._id == channel.Id, update);
            }
            Console.WriteLine("Check Database");
        }

        public static async Task CreateMongoDevice(string deviceName) {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase(deviceName+"_data");
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .FirstOrDefault(e => e.Identifier == deviceName);
            if(device is not null) {
                var deviceItems = database.GetCollection<MonitorDevice>("device_items");
                MonitorDevice deviceConfig = new MonitorDevice();
                deviceConfig.Created = DateTime.Now;
                deviceConfig.identifier = device.Identifier;
                deviceConfig.NetworkConfiguration = device.NetworkConfiguration;
                await deviceItems.InsertOneAsync(deviceConfig);
            } else {
                Console.WriteLine("Error: Device not found");
            }
        }

        public static void TestRecordList(IList<Test> modify) {
            for(int i = 0; i < modify.Count; i++) {
                modify[i].Value += 1;
            }
        }

        public static async Task TestAlerts() {
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
            collectionNames.Add(typeof(MonitorDevice), "device_items");

            var repo = new MonitorDataService("mongodb://172.20.3.30","epi1_data_test", collectionNames);
            var alertService = new AlertService("mongodb://172.20.3.30", "epi1_data_test", collectionNames[typeof(ActionItem)], collectionNames[typeof(MonitorAlert)]);
            await repo.LoadAsync();
            await alertService.Initialize();
            var itemAlerts = repo.MonitorAlerts.Select(alert => new AlertRecord(alert, ActionType.Okay)).ToList();
            DateTime now = DateTime.Now;
            int count = 0;
            while (true) {
                await alertService.ProcessAlerts(itemAlerts,now);
                var alertRecord=itemAlerts.FirstOrDefault(e => e.ChannelId==48);
                if (count == 0) {
                    alertRecord.CurrentState = ActionType.SoftWarn;
                    alertRecord.ChannelReading = 100.0f;
                    count++;
                } else if (count == 1) {
                    alertRecord.CurrentState = ActionType.Warning;
                    alertRecord.ChannelReading = 500.0f;
                    count++;
                } else {
                    alertRecord.CurrentState = ActionType.Alarm;
                    alertRecord.ChannelReading = 1000.0f;
                    count = 0;
                }
                await Task.Delay(1000);
            }
        }

        public static async Task RunDataLogger() {
            var datalogger = new DataLoggerWrapper();
            await datalogger.StartAsync();
            Console.WriteLine("Press q to exit");
            do {

            } while (Console.ReadKey().Key != ConsoleKey.Q);

            Console.WriteLine("Exiting program");
        }

        static async Task ClearAlert(AlertReading reading,MonitorAlert alert, IMongoCollection<MonitorAlert> alerts) {
            var update = Builders<MonitorAlert>.Update
                .Set(e => e.CurrentState, reading.state)
                .Set(e => e.latched, false);
            await alerts.UpdateOneAsync(e => e._id == alert._id, update);
        }

        //static UpdateDefinition<MonitorAlert> AlertUpdateAlreadyLatch(AlertReading reading) {
        //    return Builders<MonitorAlert>.Update
        //        .Set(e => e.lastAlarm, reading.timestamp)
        //        .Set(e => e.CurrentState, reading.state);
        //}

        //static UpdateDefinition<MonitorAlert> AlertUpdateLatch(AlertReading reading) {
        //    return Builders<MonitorAlert>.Update
        //        .Set(e => e.lastAlarm, reading.timestamp)
        //        .Set(e => e.CurrentState, reading.state)
        //        .Set(e => e.latched, true);
        //}

        public static async Task AlertUpdate(string deviceName) {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase(deviceName+"_data");
            var alertItems = database.GetCollection<MonitorAlert>("alert_items");
            using var context = new FacilityContext();
            var alerts = await context.Alerts
                .Include(e => e.InputChannel)
                .ThenInclude(e => e.ModbusDevice)
                .Where(e => e.InputChannel.ModbusDevice.Identifier == deviceName)
                .ToListAsync();

            alerts.ForEach((alert) => {
                var update = Builders<MonitorAlert>.Update
                .Set(s => s.displayName, alert.DisplayName)
                .Set(s=>s.itemType,alert.AlertItemType)
                .Set(s=>s.enabled,alert.Enabled);
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
            var e2Db = client.GetDatabase("epi2_data");
            var e1Db = client.GetDatabase("epi1_data");
            var e2AItems = e2Db.GetCollection<ActionItem>("action_items");
            var e1AItems = e1Db.GetCollection<ActionItem>("action_items");
            Dictionary<int, ActionType> actionTypeMap = new Dictionary<int, ActionType>();
            actionTypeMap.Add(1, ActionType.Okay);
            actionTypeMap.Add(2, ActionType.Alarm);
            actionTypeMap.Add(3, ActionType.Warning);
            actionTypeMap.Add(4, ActionType.SoftWarn);
            actionTypeMap.Add(5, ActionType.Maintenance);
            actionTypeMap.Add(6, ActionType.Custom);
            foreach(var item in actionTypeMap) {
                var update = Builders<ActionItem>.Update.Set(s => s.actionType, item.Value);
                var e2Item = e2AItems.FindOneAndUpdate(e => e._id == item.Key, update);
                var e1Item = e1AItems.FindOneAndUpdate(e => e._id == item.Key, update);
            }
            Console.WriteLine("Check Database");
        }

        public static void UpdateActionEmailSettings() {
            using var context = new FacilityContext();
            var actions = context.FacilityActions.ToList();
            var client = new MongoClient("mongodb://172.20.3.30");
            var epi1Db = client.GetDatabase("epi1_data");
            var epi2Db = client.GetDatabase("epi2_data");
            var e1Actions = epi1Db.GetCollection<ActionItem>("action_items");
            var e2Actions = epi2Db.GetCollection<ActionItem>("action_items");


            actions.ForEach((act) => {
                var update = Builders<ActionItem>.Update
                .Set(e => e.EmailEnabled, act.EmailEnabled)
                .Set(e => e.EmailPeriod, act.EmailPeriod);
                e1Actions.FindOneAndUpdate(e => e.actionType == act.ActionType, update);
                e2Actions.FindOneAndUpdate(e => e.actionType == act.ActionType, update);
            });
            Console.WriteLine("Check Databases");
        }

        static async Task CreateConfigDatabase(string deviceName) {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.30");
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .Select(e => new DeviceDTO() { 
                    Identifier = e.Identifier, 
                    NetworkConfiguration = e.NetworkConfiguration 
                }).FirstOrDefault(e => e.Identifier == deviceName);

            if (device != null) {
                Console.WriteLine($"Device {device.Identifier.ToLower()} found");
                var database = client.GetDatabase($"{device.Identifier.ToLower()}_data_test");

                Console.WriteLine("Creating Collections");
                await database.CreateCollectionAsync("analog_items");
                await database.CreateCollectionAsync("discrete_items");
                await database.CreateCollectionAsync("output_items");
                await database.CreateCollectionAsync("virtual_items");
                await database.CreateCollectionAsync("action_items");
                await database.CreateCollectionAsync("alert_items");
                await database.CreateCollectionAsync("device_items");

                IMongoCollection<MonitorDevice> deviceItems = database.GetCollection<MonitorDevice>("device_items");
                MonitorDevice deviceConfig = new MonitorDevice();
                deviceConfig.Created = DateTime.Now;
                deviceConfig.identifier = device.Identifier;
                deviceConfig.NetworkConfiguration = device.NetworkConfiguration;

                IMongoCollection<AnalogChannel> analogItems = database.GetCollection<AnalogChannel>("analog_items");
                var analogChannels = await context.Channels.OfType<AnalogInput>()
                    .AsNoTracking()
                    .Include(e=>e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e=>e.SystemChannel)
                    .Select(e=>new AnalogChannel() { 
                        identifier=e.DisplayName
                        ,_id=e.Id,
                        factor=10,
                        display=e.Display
                    }).ToListAsync();

                var discreteItems = database.GetCollection<DiscreteChannel>("discrete_items");
                var discreteChannels = await context.Channels.OfType<DiscreteInput>()
                    .AsNoTracking()
                    .Include(e=>e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e=>e.SystemChannel)
                    .Select(e => new DiscreteChannel() { 
                        identifier = e.DisplayName,
                        _id = e.Id,
                        display=e.Display
                    }).ToListAsync();

                IMongoCollection<OutputItem> outputItems = database.GetCollection<OutputItem>("output_items");
                var outputs = await context.Channels.OfType<OutputChannel>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e=>e.SystemChannel)
                    .Select(e => new OutputItem() { 
                        identifier = e.DisplayName, 
                        _id = e.Id,
                        display=e.Display
                    }).ToListAsync();

                IMongoCollection<ActionItem> actionItems = database.GetCollection<ActionItem>("action_items");
                var actions = await context.FacilityActions
                    .AsNoTracking()
                    .Select(e => new ActionItem() { 
                        identifier = e.ActionName, 
                        _id = e.Id,
                        EmailEnabled=e.EmailEnabled,
                        EmailPeriod=e.EmailPeriod,
                        actionType=e.ActionType,
                        display=true
                    }).ToListAsync();

                IMongoCollection<VirtualChannel> virtualItems = database.GetCollection<VirtualChannel>("virtual_items");
                var virtualChannels = await context.Channels.OfType<VirtualInput>()
                    .AsNoTracking()
                    .Include(e=>e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new VirtualChannel() { 
                        identifier = e.DisplayName, 
                        _id = e.Id,
                        display=e.Display
                    }).ToListAsync();

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
                        displayName=e.DisplayName,
                        bypassResetTime = e.BypassResetTime,
                        itemType=e.AlertItemType,
                        latched = false,
                        CurrentState = ActionType.Okay 
                    }).ToListAsync();

                await deviceItems.InsertOneAsync(deviceConfig);
                await analogItems.InsertManyAsync(analogChannels);
                await discreteItems.InsertManyAsync(discreteChannels);
                await outputItems.InsertManyAsync(outputs);
                await actionItems.InsertManyAsync(actions);
                await monitorAlerts.InsertManyAsync(alerts);
                await virtualItems.InsertManyAsync(virtualChannels);
                
                Console.WriteLine("Check database, press any key to exit");
            } else {
                Console.WriteLine("Device Not Found, Please Check Identifier");
            }
            Console.ReadKey();
        }

        public static async Task CreateReadingsDatabaseNew(string deviceName) {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.41");
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .Select(e => new DeviceDTO() { Identifier = e.Identifier, NetworkConfiguration = e.NetworkConfiguration })
                .FirstOrDefault(e => e.Identifier == deviceName);

            if(device is not null) {
                Console.WriteLine($"Device {device.Identifier} found");
                var database = client.GetDatabase($"{device.Identifier.ToLower()}_data");
                Console.WriteLine("Creating Collections");
                Console.WriteLine($"Device {device.Identifier.ToLower()} found");

                Console.WriteLine("Creating Collections");
                await database.CreateCollectionAsync("analog_items");
                await database.CreateCollectionAsync("discrete_items");
                await database.CreateCollectionAsync("output_items");
                await database.CreateCollectionAsync("virtual_items");
                await database.CreateCollectionAsync("action_items");
                await database.CreateCollectionAsync("alert_items");
                await database.CreateCollectionAsync("device_items");

                await database.CreateCollectionAsync("analog_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("discrete_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("virtual_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", granularity: TimeSeriesGranularity.Seconds)
                    });


                await database.CreateCollectionAsync("alert_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("device_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                Console.WriteLine("Collections Created, Populating configurations");

                IMongoCollection<MonitorDevice> deviceItems = database.GetCollection<MonitorDevice>("device_items");
                MonitorDevice deviceConfig = new MonitorDevice();
                deviceConfig.Created = DateTime.Now;
                deviceConfig.identifier = device.Identifier;
                deviceConfig.NetworkConfiguration = device.NetworkConfiguration;

                IMongoCollection<AnalogChannel> analogItems = database.GetCollection<AnalogChannel>("analog_items");
                var analogChannels = await context.Channels.OfType<AnalogInput>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new AnalogChannel() {
                        identifier = e.DisplayName,
                        _id = e.Id,
                        factor = 10,
                        display = e.Display
                    }).ToListAsync();

                var discreteItems = database.GetCollection<DiscreteChannel>("discrete_items");
                var discreteChannels = await context.Channels.OfType<DiscreteInput>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new DiscreteChannel() {
                        identifier = e.DisplayName,
                        _id = e.Id,
                        display = e.Display
                    }).ToListAsync();

                IMongoCollection<OutputItem> outputItems = database.GetCollection<OutputItem>("output_items");
                var outputs = await context.Channels.OfType<OutputChannel>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new OutputItem() {
                        identifier = e.DisplayName,
                        _id = e.Id,
                        display = e.Display
                    }).ToListAsync();

                IMongoCollection<ActionItem> actionItems = database.GetCollection<ActionItem>("action_items");
                var actions = await context.FacilityActions
                    .AsNoTracking()
                    .Select(e => new ActionItem() {
                        identifier = e.ActionName,
                        _id = e.Id,
                        EmailEnabled = e.EmailEnabled,
                        EmailPeriod = e.EmailPeriod,
                        actionType = e.ActionType,
                        display = true
                    }).ToListAsync();

                IMongoCollection<VirtualChannel> virtualItems = database.GetCollection<VirtualChannel>("virtual_items");
                var virtualChannels = await context.Channels.OfType<VirtualInput>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new VirtualChannel() {
                        identifier = e.DisplayName,
                        _id = e.Id,
                        display = e.Display
                    }).ToListAsync();

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
                        displayName = e.DisplayName,
                        bypassResetTime = e.BypassResetTime,
                        itemType = e.AlertItemType,
                        latched = false,
                        CurrentState = ActionType.Okay
                    }).ToListAsync();

                await deviceItems.InsertOneAsync(deviceConfig);
                await analogItems.InsertManyAsync(analogChannels);
                await discreteItems.InsertManyAsync(discreteChannels);
                await outputItems.InsertManyAsync(outputs);
                await actionItems.InsertManyAsync(actions);
                await monitorAlerts.InsertManyAsync(alerts);
                await virtualItems.InsertManyAsync(virtualChannels);
                Console.WriteLine("Check database, press any key to exit");
            } else {
                Console.WriteLine("Error: Device not found");
            }
            Console.ReadKey();
        }

        static async Task CreateReadingsDatabase(string deviceName) {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.30");
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .Select(e => new DeviceDTO() { Identifier = e.Identifier, NetworkConfiguration = e.NetworkConfiguration})
                .FirstOrDefault(e => e.Identifier == deviceName);

            if (device != null) {
                Console.WriteLine($"Device {device.Identifier} found");
                var database = client.GetDatabase($"{device.Identifier.ToLower()}_data_test");
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
                var modbusService = new ModbusService();
                var result = await modbusService.Read(networkConfig.IPAddress, networkConfig.Port, networkConfig.ModbusConfig);
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
                    aTempReadings.Add(new AnalogReading() { itemid = analogConfig[i]._id, value = analogInputs[i] });
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
            collectionNames.Add(typeof(MonitorDevice), "device_items");
            this._dataLogger = new ModbusDataLogger("mongodb://172.20.3.30", "epi1_data_test",collectionNames);
        }
        public async Task StartAsync() {
            Console.WriteLine("Starting Logging Service");
            await this._dataLogger.Load();
            this._timer = new Timer(this.DataLogCallback, null,TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }
        public async void DataLogCallback(object state) {
            Console.WriteLine($"{DateTime.Now}: Logged Data");
            await this._dataLogger.Read();
        }
    }
}
