using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringData.Infrastructure.Model;
using MonitoringSystem.Shared.Data;

namespace MonitoringSystem.ConsoleTesting {
    public class Program {
        static async Task Main(string[] args) {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("epi1_data_test");

            //Console.WriteLine("Creating Collections");
            //await database.CreateCollectionAsync("analog_items");
            //await database.CreateCollectionAsync("discrete_items");
            //await database.CreateCollectionAsync("output_items");
            //await database.CreateCollectionAsync("virtual_items");
            //await database.CreateCollectionAsync("action_items");
            //await database.CreateCollectionAsync("alert_items");
            //await database.CreateCollectionAsync("device_items");

            //await database.CreateCollectionAsync("analog_readings",
            //    new CreateCollectionOptions() { 
            //        TimeSeriesOptions=new TimeSeriesOptions("timestamp","itemid",granularity:TimeSeriesGranularity.Seconds)
            //    });

            //await database.CreateCollectionAsync("discrete_readings",
            //    new CreateCollectionOptions() {
            //    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
            //});

            //await database.CreateCollectionAsync("output_readings",
            //    new CreateCollectionOptions() {
            //    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
            //});

            //await database.CreateCollectionAsync("virtual_readings",
            //    new CreateCollectionOptions() {
            //    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
            //});

            //await database.CreateCollectionAsync("action_readings",
            //    new CreateCollectionOptions() {
            //    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
            //});

            //await database.CreateCollectionAsync("alert_readings",
            //    new CreateCollectionOptions() {
            //    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
            //});

            //await database.CreateCollectionAsync("device_readings",
            //    new CreateCollectionOptions() {
            //    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
            //});


            //Console.WriteLine("Gettting Collections");
            //var analogReadings = database.GetCollection<AnalogReading>("analog_readings");
            //var discreteReadings = database.GetCollection<DiscreteReading>("discrete_readings");
            //var outputReadings = database.GetCollection<OutputReading>("output_readings");
            //var virtualReadings = database.GetCollection<VirtualReading>("virtual_readings");
            //var actionReadings = database.GetCollection<ActionReading>("action_readings");
            //var alertReadings = database.GetCollection<AlertReading>("alert_readings");
            //var deviceReadings = database.GetCollection<DeviceReading>("device_readings");

            //Console.WriteLine("Creating TimeSeries Indices");

            //analogReadings.Indexes.CreateOne(new CreateIndexModel<AnalogReading>(Builders<AnalogReading>.IndexKeys.Ascending(x => x.itemid),
            //    new CreateIndexOptions()),
            //    new CreateOneIndexOptions());

            //discreteReadings.Indexes.CreateOne(new CreateIndexModel<DiscreteReading>(Builders<DiscreteReading>.IndexKeys.Ascending(x => x.itemid),
            //    new CreateIndexOptions()),
            //    new CreateOneIndexOptions());

            //outputReadings.Indexes.CreateOne(new CreateIndexModel<OutputReading>(Builders<OutputReading>.IndexKeys.Ascending(x => x.itemid),
            //    new CreateIndexOptions()),
            //    new CreateOneIndexOptions());

            //virtualReadings.Indexes.CreateOne(new CreateIndexModel<VirtualReading>(Builders<VirtualReading>.IndexKeys.Ascending(x => x.itemid),
            //    new CreateIndexOptions()),
            //    new CreateOneIndexOptions());

            //alertReadings.Indexes.CreateOne(new CreateIndexModel<AlertReading>(Builders<AlertReading>.IndexKeys.Ascending(x => x.itemid),
            //    new CreateIndexOptions()),
            //    new CreateOneIndexOptions());

            //actionReadings.Indexes.CreateOne(new CreateIndexModel<ActionReading>(Builders<ActionReading>.IndexKeys.Ascending(x => x.itemid),
            //    new CreateIndexOptions()),
            //    new CreateOneIndexOptions());

            //deviceReadings.Indexes.CreateOne(new CreateIndexModel<DeviceReading>(Builders<DeviceReading>.IndexKeys.Ascending(x => x.itemid),
            //new CreateIndexOptions()),
            //new CreateOneIndexOptions());



            //await analogReadings.InsertOneAsync(new AnalogReading() { itemid = -1, timestamp = DateTime.Now, value = 0 });
            //await discreteReadings.InsertOneAsync(new DiscreteReading() { itemid = -1, timestamp = DateTime.Now, value = false });
            //await outputReadings.InsertOneAsync(new OutputReading() { itemid = -1, timestamp = DateTime.Now, value = false });
            //await virtualReadings.InsertOneAsync(new VirtualReading() { itemid = -1, timestamp = DateTime.Now, value = false });
            //await alertReadings.InsertOneAsync(new AlertReading() { itemid = -1, timestamp = DateTime.Now, value = ActionType.Custom });
            //await actionReadings.InsertOneAsync(new ActionReading() { itemid = -1, timestamp = DateTime.Now, value = false });
            //await deviceReadings.InsertOneAsync(new DeviceReading() { itemid = -1,timestamp=DateTime.Now,value=DeviceState.OKAY });

            var analogItem = database.GetCollection<AnalogChannel>("analog_items");

            //await analogItem.InsertOneAsync(new AnalogChannel() {identifier="testChannel"});

            ///await analogReadings.Dele();

            Console.WriteLine("Should be created, check database");


        }
    }
}
