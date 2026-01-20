using System.Text;
using ClosedXML.Excel;
using MongoDB.Bson;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Extensions;
using MonitoringSystem.Shared.Services;
using MonitoringWeb.WebApp.Data;

namespace MonitoringWeb.WebApp.Services;

public class PlotDataService {
    private IMongoCollection<AnalogReadings> _analogReadings;
    private IMongoCollection<AnalogItem> _analogItems;
    private readonly IMongoClient _client;
    private readonly WebsiteConfigurationProvider _configProvider;

    public PlotDataService(IMongoClient client, WebsiteConfigurationProvider configurationProvider) {
        this._client = client;
        this._configProvider = configurationProvider;
    }

    public async Task<SensorType?> GetChannelSensor(string deviceData, ObjectId channelId) {
        var database = this._client.GetDatabase(deviceData);
        this._analogItems = database.GetCollection<AnalogItem>("analog_items");
        var channel = await this._analogItems.Find(e => e._id == channelId).FirstOrDefaultAsync();
        if (channel != null) {
            return this._configProvider.Sensors.FirstOrDefault(e => e._id == channel.SensorId);
        } else {
            return null;
        }
    }

    public async Task<IEnumerable<AnalogReadingDto>> GetData(string deviceData, DateTime start, DateTime stop) {
        /*var client = new MongoClient("mongodb://172.20.3.41");*/
        var database = this._client.GetDatabase(deviceData);
        this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
        this._analogItems = database.GetCollection<AnalogItem>("analog_items");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        var analogItems = await this._analogItems.Find(e => e.Display && e.Identifier.Contains("H2 PPM"))
            .ToListAsync();
        using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
        while (await cursor.MoveNextAsync()) {
            var batch = cursor.Current;
            foreach (var readings in batch) {
                foreach (var aItem in analogItems) {
                    var reading = readings.readings.FirstOrDefault(e => e.MonitorItemId == aItem._id);
                    if (reading != null) {
                        var aReading = new AnalogReadingDto() {
                            Name = aItem.Identifier,
                            TimeStamp = readings.timestamp.ToLocalTime(),
                            Value = reading.Value
                        };
                        analogReadings.Add(aReading);
                    }
                }
            }
        }

        return analogReadings;
    }

    public async Task<byte[]> GetThDownloadData(string deviceData, DateTime start, DateTime stop) {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase(deviceData);

        var analogItems = database.GetCollection<AnalogItem>("analog_items").Find(e => e.Display == true).ToList();
        var analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
        var aReadings = await analogReadings.Find(e => e.timestamp >= start && e.timestamp <= stop)
            .ToListAsync();
        var headers = analogItems.Select(e => e.Identifier).ToList();
        var wb = new XLWorkbook();
        var worksheet = wb.Worksheets.Add("TH Data");
        worksheet.AddPicture(@"/app/wwwroot/images/GasDetectorMap.png").MoveTo(worksheet.Cell("O2"));
        worksheet.Cell(1, 1).Value = "timestamp";
        for (int i = 0; i < headers.Count(); i++) {
            worksheet.Cell(1, i + 2).Value = headers[i];
        }

        int colCount = 1;
        int rowCount = 2;
        foreach (var readings in aReadings) {
            StringBuilder builder = new StringBuilder();
            if (readings.timestamp.IsDaylightSavingTime()) {
                worksheet.Cell(rowCount, colCount).Value = readings.timestamp.AddHours(-4).ToString();
            } else {
                worksheet.Cell(rowCount, colCount).Value = readings.timestamp.AddHours(-5).ToString();
            }

            colCount += 1;
            foreach (var reading in readings.readings) {
                worksheet.Cell(rowCount, colCount).Value = reading.Value;
                colCount += 1;
            }

            rowCount += 1;
            colCount = 1;
        }

        var stream = new MemoryStream();
        wb.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> GetBulkGasDownloadData(IEnumerable<AnalogReadingDto>? plotData, BulkGasType gasType) {
        var wb = new XLWorkbook();
        var worksheet = wb.Worksheets.Add(gasType.ToString() + "_Data");
        worksheet.Cell(1, 1).Value = "timestamp";
        worksheet.Cell(1, 2).Value = gasType.ToString();
        int colCount = 1;
        int rowCount = 2;
        foreach (var readings in plotData) {
            StringBuilder builder = new StringBuilder();
            worksheet.Cell(rowCount, 1).Value = readings.TimeStamp;
            /*if (readings.TimeStamp.IsDaylightSavingTime()) {
                worksheet.Cell(rowCount, 1).Value = readings.TimeStamp.AddHours(-4).ToString();
            } else {
                worksheet.Cell(rowCount, 1).Value = readings.TimeStamp.AddHours(-5).ToString();
            }*/
            worksheet.Cell(rowCount, 2).Value = readings.Value;
            rowCount += 1;
        }

        var stream = new MemoryStream();
        wb.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<IEnumerable<AnalogReadingDto>> GetData(string deviceData, ObjectId sensorId, DateTime start,
        DateTime stop) {
        var database = this._client.GetDatabase(deviceData);
        this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
        this._analogItems = database.GetCollection<AnalogItem>("analog_items");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        var analogItems = await this._analogItems.Find(e => e.Display && e.SensorId == sensorId)
            .ToListAsync();
        using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
        while (await cursor.MoveNextAsync()) {
            var batch = cursor.Current;
            foreach (var readings in batch) {
                foreach (var aItem in analogItems) {
                    var reading = readings.readings.FirstOrDefault(e => e.MonitorItemId == aItem._id);
                    if (reading != null) {
                        var aReading = new AnalogReadingDto() {
                            Name = aItem.Identifier,
                            TimeStamp = readings.timestamp.ToLocalTime(),
                            Value = reading.Value
                        };
                        analogReadings.Add(aReading);
                    }
                }
            }
        }

        return analogReadings;
    }
    
    public async Task<PlotData> GetChannelData(string deviceData, string chName, DateTime start, DateTime stop) {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase(deviceData);
        this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
        this._analogItems = database.GetCollection<AnalogItem>("analog_items");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        var channel = await this._analogItems.Find(e => e.Identifier == chName).FirstOrDefaultAsync();
        var sensor = this._configProvider.Sensors.FirstOrDefault(e => e._id == channel.SensorId);
        int nth = 1;
        var days = (stop - start).Days;
        if (days >= 30) {
            var n = (stop - start).Days * 0.0549 - 0.0549;
            nth = (int)Math.Round(n, 0);
            var pipeline = PipelineDefinition<AnalogReadings, BsonDocument>.Create(new BsonDocument[] {
                new BsonDocument("$match", new BsonDocument {
                    { "timestamp", new BsonDocument { { "$gte", start }, { "$lte", stop } } },
                }),
                new BsonDocument("$sort", new BsonDocument("timestamp", 1)),
                new BsonDocument("$setWindowFields", new BsonDocument {
                    { "sortBy", new BsonDocument("timestamp", 1) }, {
                        "output", new BsonDocument {
                            { "index", new BsonDocument("$rank", new BsonDocument()) }
                        }
                    }
                }),
                new BsonDocument("$match", new BsonDocument("$expr",
                    new BsonDocument("$eq", new BsonArray {
                        new BsonDocument("$mod", new BsonArray { "$index", nth }), 1
                    })))
            });
            var cursor = await _analogReadings.AggregateAsync(pipeline, new AggregateOptions { AllowDiskUse = true });
            while (await cursor.MoveNextAsync()) {
                var batch = cursor.Current;
                foreach (var readings in batch) {
                    var reading = readings["readings"].AsBsonArray
                        .FirstOrDefault(e => e["MonitorItemId"].AsObjectId == channel._id);
                    if (reading != null) {
                        var aReading = new AnalogReadingDto() {
                            Name = channel.Identifier,
                            Value = reading["Value"].AsDouble
                        };
                        aReading.TimeStamp = readings["timestamp"].ToUniversalTime().ToLocalTime();
                        /*if (aReading.TimeStamp.IsDaylightSavingTime()) {
                            aReading.TimeStamp = readings.timestamp.AddHours(-4);
                        } else {
                            aReading.TimeStamp = readings.timestamp.AddHours(-5);
                        }*/
                        aReading.Time = double.Parse(aReading.TimeStamp.ToString("yyyyMMddHHmmss"));
                        analogReadings.Add(aReading);
                    }
                }
            }

            return new PlotData(sensor, analogReadings);
        } else {
            using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
            while (await cursor.MoveNextAsync()) {
                var batch = cursor.Current;
                foreach (var readings in batch) {
                    var reading = readings.readings.FirstOrDefault(e => e.MonitorItemId == channel._id);
                    if (reading != null) {
                        var aReading = new AnalogReadingDto() {
                            Name = channel.Identifier,
                            Value = reading.Value
                        };
                        aReading.TimeStamp = readings.timestamp;
                        /*if (aReading.TimeStamp.IsDaylightSavingTime()) {
                            aReading.TimeStamp = readings.timestamp.AddHours(-4);
                        } else {
                            aReading.TimeStamp = readings.timestamp.AddHours(-5);
                        }*/
                        aReading.Time = double.Parse(aReading.TimeStamp.ToString("yyyyMMddHHmmss"));
                        analogReadings.Add(aReading);
                    }
                }
            }

            return new PlotData(sensor, analogReadings);
        }
        
    }
    
    public async Task<PlotData> GetNH3Data(DateTime start, DateTime stop) {
        var client = new MongoClient("mongodb://172.20.3.41");
        var logDatabase = client.GetDatabase("nh3_logs");
        var weightCollection = logDatabase.GetCollection<WeightReading>("weight_readings");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        var sensor = this._configProvider.Sensors.FirstOrDefault(e => e.Name == "Weight");
        int nth = 1;
        var days = (stop - start).Days;
        if (days >= 30) {
            var n = (stop - start).Days * 0.0549 - 0.0549;
            nth = (int)Math.Round(n, 0);
            var pipeline = PipelineDefinition<WeightReading, BsonDocument>.Create(new BsonDocument[] {
                new BsonDocument("$match", new BsonDocument {
                    { "timestamp", new BsonDocument { { "$gte", start }, { "$lte", stop } } },
                    { "Value", new BsonDocument("$lte", 900) }
                }),
                new BsonDocument("$sort", new BsonDocument("timestamp", 1)),
                new BsonDocument("$setWindowFields", new BsonDocument {
                    { "sortBy", new BsonDocument("timestamp", 1) }, {
                        "output", new BsonDocument {
                            { "index", new BsonDocument("$rank", new BsonDocument()) }
                        }
                    }
                }),
                new BsonDocument("$match", new BsonDocument("$expr",
                    new BsonDocument("$eq", new BsonArray {
                        new BsonDocument("$mod", new BsonArray { "$index", nth }), 1
                    })))
            });
            var cursor = await weightCollection.AggregateAsync(pipeline, new AggregateOptions { AllowDiskUse = true });
            while (await cursor.MoveNextAsync()) {
                var batch = cursor.Current;
                foreach (var readings in batch) {
                    var aReading = new AnalogReadingDto() {
                        Name = "Toner Weight",
                        Value = readings["Value"].AsDouble
                    };
                    aReading.TimeStamp = readings["timestamp"].ToUniversalTime().ToLocalTime();
                    /*if (aReading.TimeStamp.IsDaylightSavingTime()) {
                        aReading.TimeStamp = readings.timestamp.AddHours(-4);
                    } else {
                        aReading.TimeStamp = readings.timestamp.AddHours(-5);
                    }*/
                    aReading.Time = double.Parse(aReading.TimeStamp.ToString("yyyyMMddHHmmss"));
                    analogReadings.Add(aReading);
                }
            }

            return new PlotData(sensor, analogReadings);
        } else {
            using var cursor =
                await weightCollection.FindAsync(e => e.timestamp >= start && e.timestamp <= stop && e.Value <= 900);
            while (await cursor.MoveNextAsync()) {
                var batch = cursor.Current;
                foreach (var readings in batch) {
                    var aReading = new AnalogReadingDto() {
                        Name = "Toner Weight",
                        Value = readings.Value
                    };
                    aReading.TimeStamp = readings.timestamp.ToLocalTime();
                    /*if (aReading.TimeStamp.IsDaylightSavingTime()) {
                        aReading.TimeStamp = readings.timestamp.AddHours(-4);
                    } else {
                        aReading.TimeStamp = readings.timestamp.AddHours(-5);
                    }*/
                    aReading.Time = double.Parse(aReading.TimeStamp.ToString("yyyyMMddHHmmss"));
                    analogReadings.Add(aReading);
                }
            }

            return new PlotData(sensor, analogReadings);
        }
    }

    public async Task<Tuple<SensorType, IEnumerable<AnalogReadingDto>>> GetBulkNH3(DateTime start, DateTime stop) {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("nh3_data");
        this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
        this._analogItems = database.GetCollection<AnalogItem>("analog_items");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        var tank1 = await this._analogItems.Find(e => e.Identifier == "Tank1 Weight").FirstOrDefaultAsync();
        var tank2 = await this._analogItems.Find(e => e.Identifier == "Tank2 Weight").FirstOrDefaultAsync();
        var sensor = this._configProvider.Sensors.FirstOrDefault(e => e._id == tank1.SensorId);
        using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
        while (await cursor.MoveNextAsync()) {
            var batch = cursor.Current;
            foreach (var readings in batch) {
                var reading1 = readings.readings.FirstOrDefault(e => e.MonitorItemId == tank1._id);
                var reading2 = readings.readings.FirstOrDefault(e => e.MonitorItemId == tank2._id);
                if (reading1 != null && reading2 != null) {
                    var aReading1 = new AnalogReadingDto() {
                        Name = tank1.Identifier,
                        TimeStamp = readings.timestamp.ToLocalTime(),
                        Value = reading1.Value >= 0 ? reading1.Value : 0
                    };
                    aReading1.Time = double.Parse(aReading1.TimeStamp.ToString("yyyyMMddHHmmss"));
                    analogReadings.Add(aReading1);

                    var aReading2 = new AnalogReadingDto() {
                        Name = tank2.Identifier,
                        TimeStamp = readings.timestamp.ToLocalTime(),
                        Value = reading2.Value >= 0 ? reading2.Value : 0
                    };
                    aReading2.Time = double.Parse(aReading2.TimeStamp.ToString("yyyyMMddHHmmss"));
                    analogReadings.Add(aReading2);

                    var aReading3 = new AnalogReadingDto() {
                        Name = "Combined",
                        TimeStamp = readings.timestamp.ToLocalTime(),
                        Value = aReading1.Value + aReading2.Value
                    };
                    aReading3.Time = double.Parse(aReading3.TimeStamp.ToString("yyyyMMddHHmmss"));
                    analogReadings.Add(aReading3);
                }
            }
        }

        return new Tuple<SensorType, IEnumerable<AnalogReadingDto>>(sensor, analogReadings);
    }

    public async Task<IEnumerable<AnalogReadingDto>> GetDataBySensor(string deviceData, DateTime start, DateTime stop,
        ObjectId sensorId) {
        var database = this._client.GetDatabase(deviceData);
        this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
        this._analogItems = database.GetCollection<AnalogItem>("analog_items");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        var analogItems = await this._analogItems.Find(e => e.Display && e.SensorId == sensorId)
            .ToListAsync();
        using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
        while (await cursor.MoveNextAsync()) {
            var batch = cursor.Current;
            foreach (var readings in batch) {
                foreach (var aItem in analogItems) {
                    var reading = readings.readings.FirstOrDefault(e => e.MonitorItemId == aItem._id);
                    if (reading != null) {
                        var aReading = new AnalogReadingDto() {
                            Name = aItem.Identifier,
                            TimeStamp = readings.timestamp.ToLocalTime(),
                            Value = reading.Value
                        };
                        analogReadings.Add(aReading);
                    }
                }
            }
        }

        return analogReadings;
    }

    public async Task<IEnumerable<AnalogReadingDto>> GetDataNew(string deviceData, DateTime start, DateTime stop) {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase(deviceData);
        this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
        this._analogItems = database.GetCollection<AnalogItem>("analog_items");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        var analogItems = await this._analogItems.Find(e => e.Display && e.Identifier.Contains("H2 PPM"))
            .ToListAsync();
        var readings = await this._analogReadings.Find(e => e.timestamp >= start && e.timestamp <= stop)
            .ToListAsync();
        DateTime min = readings.Min(e => e.timestamp);
        foreach (var reading in readings) {
            var delta = (reading.timestamp - min).TotalHours;
            foreach (var item in analogItems) {
                var aReading = reading.readings.FirstOrDefault(e => e.MonitorItemId == item._id);
                if (aReading != null) {
                    analogReadings.Add(new AnalogReadingDto() {
                        Time = delta,
                        TimeStamp = reading.timestamp,
                        Name = item.Identifier,
                        Value = aReading.Value
                    });
                }
            }
        }

        return analogReadings;
    }

    public async Task<IEnumerable<AnalogReadingDto>> GetData(List<string> deviceData, DateTime start, DateTime stop) {
        var client = new MongoClient("mongodb://172.20.3.41");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        foreach (var data in deviceData) {
            var readings = await this.GetData(data, start, stop);
            analogReadings.AddRange(readings);
        }

        return analogReadings;
    }

    public async Task<IEnumerable<AnalogReadingDto>> GetDataBySensor(List<string> deviceData, DateTime start,
        DateTime stop) {
        var client = new MongoClient("mongodb://172.20.3.41");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        foreach (var data in deviceData) {
            var readings = await this.GetData(data, start, stop);
            analogReadings.AddRange(readings);
        }

        return analogReadings;
    }
    
        /*public async Task<IEnumerable<AnalogReadingDto>> GetChannelData(string deviceData, string chName, DateTime start,
        DateTime stop) {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase(deviceData);
        this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
        this._analogItems = database.GetCollection<AnalogItem>("analog_items");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        var h2psi = await this._analogItems.Find(e => e.Identifier == chName).FirstOrDefaultAsync();
        using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
        while (await cursor.MoveNextAsync()) {
            var batch = cursor.Current;
            foreach (var readings in batch) {
                var reading = readings.readings.FirstOrDefault(e => e.MonitorItemId == h2psi._id);
                if (reading != null) {
                    var aReading = new AnalogReadingDto() {
                        Name = h2psi.Identifier,
                        TimeStamp = readings.timestamp.ToLocalTime(),
                        Value = reading.Value
                    };
                    aReading.Time = double.Parse(aReading.TimeStamp.ToString("yyyyMMddHHmmss"));
                    analogReadings.Add(aReading);
                }
            }
        }

        return analogReadings;
    }

    public async Task<PlotData> GetChannelDatav2(string deviceData, string chName, DateTime start, DateTime stop) {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase(deviceData);
        this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
        this._analogItems = database.GetCollection<AnalogItem>("analog_items");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        var channel = await this._analogItems.Find(e => e.Identifier == chName).FirstOrDefaultAsync();
        var sensor = this._configProvider.Sensors.FirstOrDefault(e => e._id == channel.SensorId);


        using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
        while (await cursor.MoveNextAsync()) {
            var batch = cursor.Current;
            foreach (var readings in batch) {
                var reading = readings.readings.FirstOrDefault(e => e.MonitorItemId == channel._id);
                if (reading != null) {
                    var aReading = new AnalogReadingDto() {
                        Name = channel.Identifier,
                        Value = reading.Value
                    };
                    aReading.TimeStamp = readings.timestamp;
                    /*if (aReading.TimeStamp.IsDaylightSavingTime()) {
                        aReading.TimeStamp = readings.timestamp.AddHours(-4);
                    } else {
                        aReading.TimeStamp = readings.timestamp.AddHours(-5);
                    }#1#
                    aReading.Time = double.Parse(aReading.TimeStamp.ToString("yyyyMMddHHmmss"));
                    analogReadings.Add(aReading);
                }
            }
        }

        return new PlotData(sensor, analogReadings);
    }*/
        
        /*public async Task<PlotData> GetNH3Data(DateTime start, DateTime stop) {
        var client = new MongoClient("mongodb://172.20.3.41");
        var logDatabase = client.GetDatabase("nh3_logs");
        var weightCollection = logDatabase.GetCollection<WeightReading>("weight_readings");
        List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
        var sensor = this._configProvider.Sensors.FirstOrDefault(e => e.Name == "Weight");
        using var cursor =
            await weightCollection.FindAsync(e => e.timestamp >= start && e.timestamp <= stop && e.Value <= 900);
        while (await cursor.MoveNextAsync()) {
            var batch = cursor.Current;
            foreach (var readings in batch) {
                var aReading = new AnalogReadingDto() {
                    Name = "Toner Weight",
                    Value = readings.Value
                };
                aReading.TimeStamp = readings.timestamp.ToLocalTime();
                /*if (aReading.TimeStamp.IsDaylightSavingTime()) {
                    aReading.TimeStamp = readings.timestamp.AddHours(-4);
                } else {
                    aReading.TimeStamp = readings.timestamp.AddHours(-5);
                }#1#
                aReading.Time = double.Parse(aReading.TimeStamp.ToString("yyyyMMddHHmmss"));
                analogReadings.Add(aReading);
            }
        }

        return new PlotData(sensor, analogReadings);
    }*/
}