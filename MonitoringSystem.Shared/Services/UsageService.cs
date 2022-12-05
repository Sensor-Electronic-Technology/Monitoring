﻿using System.Globalization;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.UsageModel;

namespace MonitoringSystem.Shared.Services;

public class UsageService {
    private readonly ILogger<UsageService> _logger;
    private readonly IMongoClient _client;

    public UsageService(IMongoClient client, ILogger<UsageService> logger) {
        this._logger = logger;
        this._client = client;
    }

    public UsageService() {
        this._client = new MongoClient("mongodb://172.20.3.41");
    }
    public async Task<IEnumerable<UsageDayRecord>> GetNH3Usage() {
        var database = this._client.GetDatabase("nh3_data");
        var analogCollection = database.GetCollection<AnalogItem>("analog_items");
        var analogReadCollection = database.GetCollection<AnalogReadings>("analog_readings");
        var usageCollection = database.GetCollection<UsageDayRecord>("nh3_usage");
        var item1 = await analogCollection.Find(e => e.Identifier == "Tank1 Weight").FirstOrDefaultAsync();
        var item2 = await analogCollection.Find(e => e.Identifier == "Tank2 Weight").FirstOrDefaultAsync();
        return await this.GetUsageRecords(usageCollection, analogReadCollection, item1, item2);
    }
    
    public async Task<IEnumerable<UsageDayRecord>> GetH2Usage() {
        var database = this._client.GetDatabase("epi1_data");
        var analogCollection = database.GetCollection<AnalogItem>("analog_items");
        var analogReadCollection = database.GetCollection<AnalogReadings>("analog_readings");
        var usageCollection = database.GetCollection<UsageDayRecord>("h2_usage");
        var item = await analogCollection.Find(e => e.Identifier == "H2 PSI").FirstOrDefaultAsync();
        return await this.GetUsageRecords(usageCollection, analogReadCollection, item);
    }
    
    public async Task<IEnumerable<UsageDayRecord>> GetN2Usage() {
        var database = this._client.GetDatabase("epi1_data");
        var analogCollection = database.GetCollection<AnalogItem>("analog_items");
        var analogReadCollection = database.GetCollection<AnalogReadings>("analog_readings");
        var usageCollection = database.GetCollection<UsageDayRecord>("n2_usage");
        var item = await analogCollection.Find(e => e.Identifier == "N2 inH20").FirstOrDefaultAsync();
        return await this.GetUsageRecords(usageCollection, analogReadCollection, item);
    }

    private async Task<IEnumerable<UsageDayRecord>> GetUsageRecords(
        IMongoCollection<UsageDayRecord> usageCollection,
        IMongoCollection<AnalogReadings> analogReadCollection, AnalogItem? item1, AnalogItem? item2 = null) {
        var sort = Builders<UsageDayRecord>.Sort.Descending(e => e.Date);
        var count = await usageCollection.EstimatedDocumentCountAsync();
        if (count == 0) {
            var stopDate = DateTime.Now.Date.AddHours(-5);
            List<AnalogReadings> readings;
            Dictionary<DateTime, List<ValueReturn>> days;
            readings = await analogReadCollection.Find(e=>e.timestamp<stopDate)
                .ToListAsync();
            if (item2 != null) {
                days = readings.GroupBy(e =>
                            e.timestamp.Date,
                        e => new ValueReturn() {
                            timestamp = e.timestamp,
                            value = (e.readings.FirstOrDefault(m => m.MonitorItemId == item1._id)!.Value +
                            e.readings.FirstOrDefault(m => m.MonitorItemId == item2._id)!.Value)
                        })
                    .ToDictionary(e => e.Key, e => e.ToList());
            } else {
                days = readings.GroupBy(e =>
                            e.timestamp.Date,
                        e => new ValueReturn() {
                            timestamp = e.timestamp,
                            value = e.readings.FirstOrDefault(m => m.MonitorItemId == item1._id)!.Value
                        })
                    .ToDictionary(e => e.Key, e => e.ToList());
            }
            List<UsageDayRecord> dayRecords = new List<UsageDayRecord>();
            foreach (var day in days) {
                UsageDayRecord usageDayRecord = new UsageDayRecord() {
                    _id = ObjectId.GenerateNewId(),
                    DayOfMonth = day.Key.Day,
                    DayOfWeek = day.Key.DayOfWeek,
                    Date = day.Key,
                    Month = day.Key.Month,
                    WeekOfYear =
                        CultureInfo.CurrentCulture.DateTimeFormat.Calendar.GetWeekOfYear(day.Key,
                            CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                    Year = day.Key.Year,
                    DayOfYear = day.Key.DayOfYear,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(day.Key.Month)
                };
                usageDayRecord.ChannelIds.Add(item1!._id);
                if (item2 != null) {
                    usageDayRecord.ChannelIds.Add(item2._id);
                }

                var hours = day.Value.GroupBy(e => e.timestamp.Hour).ToDictionary(e=>e.Key,e=>e.ToList());

                List<double> hourRates = new List<double>();
                foreach (var hour in hours) {
                    var first = hour.Value.First();
                    var last = hour.Value.Last();
                    var rate = (last.value - first.value)/((last.timestamp-first.timestamp).TotalHours);
                    rate = (rate > 0) ? 0 : rate;
                    hourRates.Add(rate);
                }
                
                /*List<double> rates = new List<double>();
                for (int i = 1; i < day.Value.Count(); i++) {
                    var dlb = (day.Value[i].value - day.Value[i-1].value);
                    var dt = (day.Value[i].timestamp - day.Value[i-1].timestamp).TotalMinutes;
                    if (dt != 0) {
                        var rate = dlb / dt;
                        if (rate <= 100) {
                            rates.Add(rate);
                        }
                    }
                }*/
                usageDayRecord.PerHour = Math.Abs(hourRates.Average());
                usageDayRecord.PerMin = usageDayRecord.PerHour / 60;
                usageDayRecord.Usage = usageDayRecord.PerHour * 24;
                dayRecords.Add(usageDayRecord);
            }
            await usageCollection.InsertManyAsync(dayRecords);
            return dayRecords.AsEnumerable();
            
        } else {
            var latest = await usageCollection.Find(_ => true).Sort(sort).FirstAsync();
            var deltaHours = (DateTime.Now - latest.Date.AddDays(1)).TotalHours;
            var now = DateTime.Now.Date;
            if (deltaHours >= 24) {
                var start = latest.Date.AddDays(1);
                var stop = DateTime.Now.Date.AddSeconds(-1).AddHours(-5);
                List<AnalogReadings> readings;
                Dictionary<DateTime, List<ValueReturn>> days;
                if (item2 != null) {
                    readings = await analogReadCollection.Find(e =>
                            e.timestamp >= start &&
                            e.timestamp < stop)
                        .ToListAsync();
                    days = readings.GroupBy(e =>
                                e.timestamp.Date,
                            e => new ValueReturn() {
                                timestamp = e.timestamp,
                                value = (e.readings.FirstOrDefault(m => m.MonitorItemId == item1._id)!.Value +
                                e.readings.FirstOrDefault(m => m.MonitorItemId == item2._id)!.Value)
                            })
                        .ToDictionary(e => e.Key, e => e.ToList());
                } else {
                    readings = await analogReadCollection.Find(e =>
                            e.timestamp >= start &&
                            e.timestamp < stop)
                        .ToListAsync();
                    days = readings.GroupBy(e =>
                                e.timestamp.Date,
                            e => new ValueReturn() {
                                timestamp = e.timestamp,
                                value = e.readings.FirstOrDefault(m => m.MonitorItemId == item1._id)!.Value
                            })
                        .ToDictionary(e => e.Key, e => e.ToList());
                }
                List<UsageDayRecord> dayRecords = new List<UsageDayRecord>();
                foreach (var day in days) {
                    UsageDayRecord usageDayRecord = new UsageDayRecord() {
                        _id = ObjectId.GenerateNewId(),
                        DayOfMonth = day.Key.Day,
                        DayOfWeek = day.Key.DayOfWeek,
                        Date = day.Key,
                        Month = day.Key.Month,
                        WeekOfYear =
                            CultureInfo.CurrentCulture.DateTimeFormat.Calendar.GetWeekOfYear(day.Key,
                                CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                        Year = day.Key.Year,
                        DayOfYear = day.Key.DayOfYear,
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(day.Key.Month)
                    };
                    usageDayRecord.ChannelIds.Add(item1!._id);
                    if (item2 != null) {
                        usageDayRecord.ChannelIds.Add(item2._id);
                    }

                    var hours = day.Value.GroupBy(e => e.timestamp.Hour).ToDictionary(e=>e.Key,e=>e.ToList());
                    
                    List<double> rates = new List<double>();
                    for (int i = 1; i < day.Value.Count(); i++) {
                        var dlb = (day.Value[i].value - day.Value[i-1].value);
                        var dt = (day.Value[i].timestamp - day.Value[i-1].timestamp).TotalMinutes;
                        if (dt != 0) {
                            var rate = dlb / dt;
                            if (rate <= 100) {
                                rates.Add(rate);
                            }
                        }
                    }
                    /*usageDayRecord.PerHour = Math.Abs(hourRates.Average());
                    usageDayRecord.PerMin = usageDayRecord.PerHour / 60;
                    usageDayRecord.Usage = usageDayRecord.PerHour * 24;*/
                    dayRecords.Add(usageDayRecord);
                }
                await usageCollection.InsertManyAsync(dayRecords);
                return dayRecords.AsEnumerable();
            } else {
                return (await usageCollection.Find(_ => true).ToListAsync()).AsEnumerable();
            }
        }
    }
}

public struct ValueReturn {
    public DateTime timestamp { get; set; }
    public double value { get; set; }
}