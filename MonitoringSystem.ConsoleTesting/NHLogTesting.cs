using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MongoDB.Bson;
using NWaves.Filters;
using MathNet.Filtering;
using MonitoringSystem.Shared.Data.LogModel;
using NWaves.Filters.Base64;
using NWaves.Filters.Base;
using NWaves.Signals;
using NWaves.Utils;


namespace MonitoringSystem.ConsoleTesting; 

public class NHLogTesting {
    
    static async Task Main(string[] args) {
        //await CreateNHLogDatabase();
        //await AggregateTest();
        //await CreateCalibrations();
        //await AddChannelName();
        await CreateWeightReadings();
        await PopulateWeightReadings();
        //await OutputWeightReading();
        //await OutputWeightRaw();
    }

    static async Task OutputWeightReading() {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("nh3_data");
        var logDatabase = client.GetDatabase("nh3_logs");
        var weightCollection = logDatabase.GetCollection<WeightReading>("weight_readings");
        List<WeightReading> weightReadings = new List<WeightReading>();
        /*var start = new DateTime(2023, 11, 27, 12, 0, 0);
        var stop = DateTime.Now;*/
        StringBuilder builder = new StringBuilder();
        List<Tuple<DateTime, float>> values = new List<Tuple<DateTime, Single>>();
        var allWeights = await weightCollection.Find(_ => true).ToListAsync();
        var weights=allWeights.Select(e => new { e.timestamp, e.Value}).ToList();
        List<Single> floats = new List<Single>();
        foreach (var weight in weights) {
            floats.Add(Convert.ToSingle(weight.Value));
        }

        /*builder.AppendFormat($"{readings.timestamp}\t{readings.Value}").AppendLine();*/
        
        var filter = new MedianFilter2(9);
        var signal = new DiscreteSignal(1, floats.ToArray());
        var processed=filter.ApplyTo(signal);

        for (int i = 0; i < weights.Count; i++) {
            builder.AppendFormat($"{weights[i].timestamp}\t{processed[i]}").AppendLine();
        }
        
        await File.WriteAllTextAsync(@"C:\Users\aelmendo\Documents\weights3.txt", builder.ToString());
    }

    static async Task OutputWeightRaw() {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("nh3_data");
        var logDatabase = client.GetDatabase("nh3_logs");
        var weightCollection = logDatabase.GetCollection<WeightReading>("weight_readings");
        var analogReadingCollection = database.GetCollection<AnalogReadings>("analog_readings");
        var analogItemCollection = database.GetCollection<AnalogItem>("analog_items");
        List<WeightReading> weightReadings = new List<WeightReading>();
        var start = new DateTime(2023, 11, 1, 12, 0, 0);
        var stop = DateTime.Now;
        var channel = await analogItemCollection.Find(e => e.Identifier == "Tank1 Weight").FirstOrDefaultAsync();
        using var cursor = await analogReadingCollection.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
        StringBuilder builder = new StringBuilder();
        while (await cursor.MoveNextAsync()) {
            var batch = cursor.Current;
            foreach (var readings in batch) {
                var reading =readings.readings.FirstOrDefault(e=>e.MonitorItemId==channel._id);
                if (reading != null) {
                    builder.AppendFormat($"{readings.timestamp}\t{reading.Value}").AppendLine();
                    /*weightReadings.Add(new WeightReading() {
                        _id=ObjectId.GenerateNewId(),
                        timestamp = readings.timestamp,
                        ChannelName = "Tank1 Weight",
                        Value=reading.Value
                    });*/
                }
            }
        }
        await File.WriteAllTextAsync(@"C:\Users\aelmendo\Documents\weightsall.txt", builder.ToString());
        //await weightCollection.InsertManyAsync(weightReadings);
    }

    static async Task PopulateWeightReadings() {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("nh3_data");
        var logDatabase = client.GetDatabase("nh3_logs");
        var weightCollection = logDatabase.GetCollection<WeightReading>("weight_readings");
        var analogReadingCollection = database.GetCollection<AnalogReadings>("analog_readings");
        var analogItemCollection = database.GetCollection<AnalogItem>("analog_items");
        List<WeightReading> weightReadings = new List<WeightReading>();
        var start = new DateTime(2023, 11, 19, 12, 0, 0);
        var stop = DateTime.Now;
        var channel = await analogItemCollection.Find(e => e.Identifier == "Tank1 Weight").FirstOrDefaultAsync();
        using var cursor = await analogReadingCollection.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
        while (await cursor.MoveNextAsync()) {
            var batch = cursor.Current;
            foreach (var readings in batch) {
                var reading =readings.readings.FirstOrDefault(e=>e.MonitorItemId==channel._id);
                if (reading != null) {
                    weightReadings.Add(new WeightReading() {
                        _id=ObjectId.GenerateNewId(),
                        timestamp = readings.timestamp,
                        ChannelName = "Tank1 Weight",
                        Value=reading.Value
                    });
                }
            }
        }
        await weightCollection.InsertManyAsync(weightReadings);
    }

    static async Task CreateWeightReadings() {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("nh3_logs");
        await database.CreateCollectionAsync("weight_readings",
        new CreateCollectionOptions() {
            TimeSeriesOptions = new TimeSeriesOptions("timestamp", granularity: TimeSeriesGranularity.Seconds)
        });
        Console.WriteLine("Check Database");
    }

    static async Task AddChannelName() {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("nh3_logs");
        var scaleCollection = database.GetCollection<TankScale>("tank_scales");

        var tankScales = await scaleCollection.Find(_ => true).ToListAsync();
        foreach (var scale in tankScales) {
        var channelName = $"Tank{scale.ScaleId} Weight";
        var update = Builders<TankScale>.Update.Set(e => e.ChannelName, channelName);
        var filter = Builders<TankScale>.Filter.Eq(e => e._id, scale._id);
        await scaleCollection.UpdateOneAsync(filter, update);
        }
        Console.WriteLine("Check Database");
    }

    static async Task CreateCalibrations() {
        Console.WriteLine("Create NH Database");
        var client = new MongoClient("mongodb://172.20.3.41");
        var database=client.GetDatabase("nh3_logs");
        var scaleCollection = database.GetCollection<TankScale>("tank_scales");

        var scale1 = new TankScale() {
        _id = ObjectId.GenerateNewId(),
        ScaleId = 1,
        TankScaleState = TankScaleState.InUse
        };
        var scale2 = new TankScale() {
        _id = ObjectId.GenerateNewId(),
        ScaleId = 2,
        TankScaleState = TankScaleState.IdleOnScaleMeasured
        };
        var scale3 = new TankScale() {
            _id = ObjectId.GenerateNewId(),
            ScaleId = 3,
            TankScaleState = TankScaleState.NoTank
        };
        var scale4 = new TankScale() {
            _id = ObjectId.GenerateNewId(),
            ScaleId = 4,
            TankScaleState = TankScaleState.IdleOnScaleNotMeasured
        };
        var scale1Calibration = new Calibration() {
            CalibrationDate = DateTime.Now.AddDays(-5),
            ZeroValue = 0,
            NonZeroValue = 100,
            ZeroRawValue = 576620,
            NonZeroRawValue = 658736,
            IsCurrent = true
        };
        var scale2Calibration = new Calibration() {
            CalibrationDate = DateTime.Now.AddDays(-5),
            ZeroValue = 0,
            NonZeroValue = 100,
            ZeroRawValue = 581616,
            NonZeroRawValue = 663909,
            IsCurrent = true
        };
        var scale3Calibration = new Calibration() {
            CalibrationDate = DateTime.Now,
            ZeroRawValue = 588465,
            NonZeroRawValue = 610038,
            ZeroValue = 0,
            NonZeroValue = 30,
            IsCurrent = true
        };

        var scale4Calibration = new Calibration() {
            CalibrationDate = DateTime.Now,
            ZeroRawValue = 1069977,
            NonZeroRawValue = 1113364,
            ZeroValue = 0,
            NonZeroValue = 26,
            IsCurrent = true
        };

        scale1.CurrentCalibration = scale1Calibration;
        scale2.CurrentCalibration = scale2Calibration;
        scale3.CurrentCalibration = scale3Calibration;
        scale4.CurrentCalibration = scale4Calibration;

        scale1.CalibrationLog.Add(scale1Calibration);
        scale2.CalibrationLog.Add(scale2Calibration);
        scale3.CalibrationLog.Add(scale3Calibration);
        scale4.CalibrationLog.Add(scale4Calibration);

        var tank1 = new NH3Tank() {
        StartDate = new DateTime(2023,8,1),
        SerialNumber = "1234s",
        StartWeight = 883,
        };

        var tank2 = new NH3Tank() {
        StartDate = new DateTime(2023,10,1),
        SerialNumber = "0923s",
        StartWeight = 884
        };

        var tank4 = new NH3Tank() {
        StartDate = new DateTime(2023,10,1),
        SerialNumber = "0923s",
        StartWeight = 884,
        };

        var tank1Measured = new TankWeight() {
        Gas = 883,
        Gross = 2614,
        Tare = 1732
        };

        var tank2Measured = new TankWeight() {
        Gas = 884,
        Gross = 2625,
        Tare = 1741
        };

        tank1.MeasuredWeight = tank1Measured;
        tank1.LabeledWeight = tank1Measured;

        tank2.MeasuredWeight = tank2Measured;
        tank2.LabeledWeight = tank2Measured;

        scale1.CurrentTank = tank1;
        scale2.CurrentTank = tank2;
        scale4.CurrentTank = tank4;

        await scaleCollection.InsertOneAsync(scale1);
        await scaleCollection.InsertOneAsync(scale2);
        await scaleCollection.InsertOneAsync(scale3);
        await scaleCollection.InsertOneAsync(scale4);
        Console.WriteLine("Check Database");
    }

    static async Task AggregateTest() {
        Console.WriteLine("Create NH Database");
        var client = new MongoClient("mongodb://172.20.3.41");
        var database=client.GetDatabase("nh3_tank_logs");
        var tankCollection = database.GetCollection<NH3Tank>("nh3_tanks");
        var scaleCollection = database.GetCollection<TankScale>("tank_scales");


        //scaleCollection.Aggregate()
        //scaleCollection.Aggregate()
        /*var stage1 =
        new BsonDocument("$lookup",
        new BsonDocument {
        { "from", "tank_scales" },
        { "localField", "TankScaleId" },
        { "foreignField", "_id" },
        { "as", "scale" }
        });
        var stage2 =
        new BsonDocument("$unwind",
        new BsonDocument("path", "$scale"));


        var results=tankCollection.Aggregate().AppendStage<BsonDocument>(stage1)
        .AppendStage<BsonDocument>(stage2).ToEnumerable();
        foreach (var result in results) {
        Console.WriteLine(result);
        }*/
    }

    static async Task CreateNHLogDatabase() {
        Console.WriteLine("Create NH Database");
        var client = new MongoClient("mongodb://172.20.3.41");
        var database=client.GetDatabase("nh3_tank_logs");
        var tankCollection = database.GetCollection<NH3Tank>("nh3_tanks");
        var scaleCollection = database.GetCollection<TankScale>("tank_scales");
        var scale1 = new TankScale() {
            _id = ObjectId.GenerateNewId(),
            ScaleId = 1,
        };
        var scale2 = new TankScale() {
            _id = ObjectId.GenerateNewId(),
            ScaleId = 1,
        };
        var tank1 = new NH3Tank() {


            StartDate = new DateTime(2023,8,1),
            StopDate = new DateTime(2023,10,1),

            SerialNumber = "1234s",
            StartWeight = 883,
            StopWeight = 85
        };
        var tank2 = new NH3Tank() {


            StartDate = new DateTime(2023,10,1),

            SerialNumber = "0923s",
            StartWeight = 883
        };

        await scaleCollection.InsertOneAsync(scale1);
        await scaleCollection.InsertOneAsync(scale2);
        
        await tankCollection.InsertOneAsync(tank1);
        await tankCollection.InsertOneAsync(tank2);
        Console.WriteLine("Check Database");

    }
    
}

















