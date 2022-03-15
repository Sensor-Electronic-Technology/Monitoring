using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringData.Infrastructure.Model;
using MonitoringSystem.Shared.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MonitoringSystem.ConsoleTesting {
    public class Program {
        static async Task Main(string[] args) {
            await CreateConfigDatabse();
        }

        static async Task CreateConfigDatabse() {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.30");
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .Select(e => new DeviceDTO() { Identifier = e.Identifier, NetworkConfiguration = e.NetworkConfiguration })
                .FirstOrDefault(e => e.Identifier == "epi1");

            if (device != null) {
                Console.WriteLine($"Device {device.Identifier} found");
                var database = client.GetDatabase($"{device.Identifier}_data_testing");

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
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("epi1_data");

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
        }
    }
}
