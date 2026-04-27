using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.UsageModel;

namespace MonitoringSystem.Shared.Services;

public class UsageDataService {
    private readonly ILogger<UsageDataService> _logger;
    private readonly IMongoClient _client;
    
    public UsageDataService(IMongoClient client, ILogger<UsageDataService> logger) {
        this._logger = logger;
        this._client = client;
    }
    
    public async Task<(double rate, double lastReading)> GetBulkH2Usage(int days, bool includeWeekends = true) {
        var database = this._client.GetDatabase("epi1_data");
        var analogCollection = database.GetCollection<AnalogItem>("analog_items");
        var analogReadCollection = database.GetCollection<AnalogReadings>("analog_readings");
        var usageCollection = database.GetCollection<UsageDayRecord>("h2_usage");
        var item = await analogCollection.Find(e => e.Identifier == "Bulk H2(PSI)").FirstOrDefaultAsync();
        var date = DateTime.Now.AddDays(-days);
        var allUsage = await this.FetchUsageData(BulkGasType.H2);
        //var allUsage = await this.GetUsageRecordsV2(usageCollection, analogReadCollection, 0, 3000, item);
        var usage = allUsage.Where(e => e.Date >= date);
        if (!includeWeekends)
            usage = usage.Where(e => e.DayOfWeek != DayOfWeek.Saturday && e.DayOfWeek != DayOfWeek.Sunday);
        var lastReadingEntry = analogReadCollection.AsQueryable()
            .OrderByDescending(r => r.timestamp)
            .FirstOrDefault();
        var lastReading = lastReadingEntry?.readings
            .Where(e => e.MonitorItemId == item._id).Select(e => e.Value)
            .FirstOrDefault() ?? 0;
        return new(usage.Average(e => e.Usage), lastReading);
    }
    
    public async Task<double> GetLastReading() {
        var database = this._client.GetDatabase("epi1_data");
        var analogCollection = database.GetCollection<AnalogItem>("analog_items");
        var analogReadCollection = database.GetCollection<AnalogReadings>("analog_readings");
        var item = await analogCollection.Find(e => e.Identifier == "Bulk H2(PSI)").FirstOrDefaultAsync();
        var lastReadingEntry = analogReadCollection.AsQueryable()
            .OrderByDescending(r => r.timestamp)
            .FirstOrDefault();
        var lastReading = lastReadingEntry?.readings
            .Where(e => e.MonitorItemId == item._id).Select(e => e.Value)
            .FirstOrDefault() ?? 0;
        return lastReading;
    }
    
    public async Task<(double rate, double lastReading)> GetBulkH2Usage(DateTime startDate, DateTime stopDate,
        bool includeWeekends = true) {
        var database = this._client.GetDatabase("epi1_data");
        var analogCollection = database.GetCollection<AnalogItem>("analog_items");
        var analogReadCollection = database.GetCollection<AnalogReadings>("analog_readings");
        var item = await analogCollection.Find(e => e.Identifier == "Bulk H2(PSI)").FirstOrDefaultAsync();
        var allUsage = await this.FetchUsageData(BulkGasType.H2, startDate, stopDate);
        var usage = allUsage.Where(e => e.Date >= startDate && e.Date <= stopDate);
        if (!includeWeekends)
            usage = usage.Where(e => e.DayOfWeek != DayOfWeek.Saturday && e.DayOfWeek != DayOfWeek.Sunday);
        var lastReadingEntry = analogReadCollection.AsQueryable()
            .OrderByDescending(r => r.timestamp)
            .FirstOrDefault();
        var lastReading = lastReadingEntry?.readings
            .Where(e => e.MonitorItemId == item._id).Select(e => e.Value)
            .FirstOrDefault() ?? 0;
        return new(usage.Average(e => e.Usage), lastReading);
    }
    
    public async Task<List<UsageDayRecord>> FetchUsageData(BulkGasType gasType, DateTime? startDate = null, DateTime? endDate = null) {
        switch (gasType) {
            case BulkGasType.H2: {
                var database = this._client.GetDatabase("epi1_data");
                var usageCollection = database.GetCollection<UsageDayRecord>("h2_usage");
                return await usageCollection.Find(_ => true).ToListAsync();
            }
            case BulkGasType.N2: {
                var database = this._client.GetDatabase("epi1_data");
                var usageCollection = database.GetCollection<UsageDayRecord>("n2_usage");
                return await usageCollection.Find(_ => true).ToListAsync();
            }
            case BulkGasType.NH3: {
                var database = this._client.GetDatabase("nh3_data");
                var usageCollection = database.GetCollection<UsageDayRecord>("nh3_usage");
                return await usageCollection.Find(_ => true).ToListAsync();
            }
            case BulkGasType.SI: {
                var database = this._client.GetDatabase("epi1_data");
                var usageCollection = database.GetCollection<UsageDayRecord>("si_usage");
                return await usageCollection.Find(_ => true).ToListAsync();
            }
            
            default:
                throw new ArgumentException("Invalid gas type");
        }
    }
}