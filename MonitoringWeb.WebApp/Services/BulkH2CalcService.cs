using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;
using MonitoringSystem.Shared.Services;
using MonitoringWeb.WebApp.Data;

namespace MonitoringWeb.WebApp.Services;

public class BulkH2CalcService {
    private readonly IMongoCollection<AnalogItem> _analogItemCollection;
    private readonly IMongoCollection<BulkH2CalcSettings> _bulkH2CalcSettingsCollection;
    private readonly UsageService _usageService;

    public BulkH2CalcService(IMongoClient client, UsageService usageService, IOptions<MonitorWebsiteSettings> options) {
        var epiDatabase = client.GetDatabase("epi1_data");
        this._analogItemCollection = epiDatabase.GetCollection<AnalogItem>("analog_items");
        var database = client.GetDatabase(options.Value.DatabaseName);
        this._bulkH2CalcSettingsCollection =
            database.GetCollection<BulkH2CalcSettings>(options.Value.BulkH2CalcSettingsCollection);
        this._usageService = usageService;
    }

    public async Task<BulkH2SettingsDto> GetSettings() {
        var bulkSettings = await this._bulkH2CalcSettingsCollection.Find(_ => true).FirstOrDefaultAsync();
        var anlogItem = await this._analogItemCollection.Find(e => e.Identifier == "Bulk H2(PSI)")
            .FirstOrDefaultAsync();

        return new BulkH2SettingsDto() {
            AnalogItem = anlogItem,
            BulkH2CalcSettings = bulkSettings ?? BulkH2CalcSettings.Init()
        };
    }
    public async Task<bool> SaveNewAlertLevels(Estimates estimates) {
        var update=Builders<AnalogItem>.Update
            .Set(e=>e.Level1SetPoint,estimates.SoftWarn)
            .Set(e=>e.Level2SetPoint,estimates.Warning)
            .Set(e=>e.Level3SetPoint,estimates.Alarm);
        var updateResult=await this._analogItemCollection.UpdateOneAsync(e => e.Identifier == "Bulk H2(PSI)", update);
        return updateResult.IsAcknowledged;
    }

    public async Task<H2AlertCalcDto> CalculateAlertLevels(int days, bool includeWeekends) {
        var usage = await this._usageService.GetBulkH2Usage(days, includeWeekends);
        var settings = await this.GetSettings();
        H2AlertCalcDto result = new H2AlertCalcDto() {
            Days = days
        };
        var estimates = new Estimates();

        result.Rate = Math.Round(usage.rate, 2);
        result.LastReading = Math.Round(usage.lastReading, 2);
        estimates.SoftWarn = Math.Round(settings.BulkH2CalcSettings.AlarmLevel + (settings.BulkH2CalcSettings.DaysFromAlarm * result.Rate), 2);
        estimates.SoftWarn=(int) Math.Ceiling(estimates.SoftWarn/10)*10;
        estimates.Warning = Math.Round(settings.BulkH2CalcSettings.AlarmLevel + (((double)settings.BulkH2CalcSettings.DaysFromAlarm/2) * result.Rate), 2);
        estimates.Warning=(int) Math.Ceiling(estimates.Warning/10)*10;
        estimates.Alarm = settings.BulkH2CalcSettings.AlarmLevel;


        estimates.SoftWarnDate = DateTime.Now.AddDays((result.LastReading - estimates.SoftWarn) / result.Rate);
        estimates.WarningDate = DateTime.Now.AddDays((result.LastReading - estimates.Warning) / result.Rate);
        estimates.AlarmDate = DateTime.Now.AddDays((result.LastReading - estimates.Alarm) / result.Rate);
        result.Estimates = estimates;
        return result;
    }
    
    public async Task<H2AlertCalcDto> CalculateAlertLevels(DateTime start,DateTime stop, bool includeWeekends) {
        int days = (int) Math.Ceiling((stop - start).TotalDays);
        var usage = await this._usageService.GetBulkH2Usage(days, includeWeekends);
        var settings = await this.GetSettings();
        H2AlertCalcDto result = new H2AlertCalcDto() {
            Days = days
        };
        var estimates = new Estimates();

        result.Rate = Math.Round(usage.rate, 2);
        result.LastReading = Math.Round(usage.lastReading, 2);
        estimates.SoftWarn = Math.Round(settings.BulkH2CalcSettings.AlarmLevel + (settings.BulkH2CalcSettings.DaysFromAlarm * result.Rate), 2);
        estimates.SoftWarn=(int) Math.Ceiling(estimates.SoftWarn/10)*10;
        estimates.Warning = Math.Round(settings.BulkH2CalcSettings.AlarmLevel + (((double)settings.BulkH2CalcSettings.DaysFromAlarm/2) * result.Rate), 2);
        estimates.Warning=(int) Math.Ceiling(estimates.Warning/10)*10;
        estimates.Alarm = settings.BulkH2CalcSettings.AlarmLevel;


        estimates.SoftWarnDate = DateTime.Now.AddDays((result.LastReading - estimates.SoftWarn) / result.Rate);
        estimates.WarningDate = DateTime.Now.AddDays((result.LastReading - estimates.Warning) / result.Rate);
        estimates.AlarmDate = DateTime.Now.AddDays((result.LastReading - estimates.Alarm) / result.Rate);
        result.Estimates = estimates;
        return result;
    }
    
    public async Task<H2AlertCalcDto> CalculateAlertLevels(double rate) {
        var settings = await this.GetSettings();
        H2AlertCalcDto result = new H2AlertCalcDto();
        var estimates = new Estimates();

        result.Rate = rate;
        result.LastReading = await this._usageService.GetLastReading();
        estimates.SoftWarn = Math.Round(settings.BulkH2CalcSettings.AlarmLevel + (settings.BulkH2CalcSettings.DaysFromAlarm * result.Rate), 2);
        estimates.SoftWarn=(int) Math.Ceiling(estimates.SoftWarn/10)*10;
        estimates.Warning = Math.Round(settings.BulkH2CalcSettings.AlarmLevel + (((double)settings.BulkH2CalcSettings.DaysFromAlarm/2) * result.Rate), 2);
        estimates.Warning=(int) Math.Ceiling(estimates.Warning/10)*10;
        estimates.Alarm = settings.BulkH2CalcSettings.AlarmLevel;


        estimates.SoftWarnDate = DateTime.Now.AddDays((result.LastReading - estimates.SoftWarn) / result.Rate);
        estimates.WarningDate = DateTime.Now.AddDays((result.LastReading - estimates.Warning) / result.Rate);
        estimates.AlarmDate = DateTime.Now.AddDays((result.LastReading - estimates.Alarm) / result.Rate);
        result.Estimates = estimates;
        return result;
    }

    public Task UpdateSettings(BulkH2CalcSettings settings) {
        var update = Builders<BulkH2CalcSettings>.Update
            .Set(e => e.DaysFromAlarm, settings.DaysFromAlarm)
            .Set(e => e.IncludeWeekends, settings.IncludeWeekends)
            .Set(e => e.MinSoftWarnLevel, settings.MinSoftWarnLevel)
            .Set(e => e.TimeStamp, DateTime.UtcNow);
        return this._bulkH2CalcSettingsCollection.UpdateOneAsync(e => e._id == settings._id, update,
            new UpdateOptions { IsUpsert = true });
    }

    public Task UpdateAnalogItem(AnalogItem? analogItem) {
        if (analogItem == null) return Task.CompletedTask;

        var update = Builders<AnalogItem>.Update
            .Set(e => e.Level1SetPoint, analogItem.Level1SetPoint)
            .Set(e => e.Level2SetPoint, analogItem.Level3SetPoint)
            .Set(e => e.Level3SetPoint, analogItem.Level3SetPoint);
        return this._analogItemCollection.UpdateOneAsync(e => e._id == analogItem._id, update);
    }

    public async Task<List<AnalogItem>> GetAnalogItems() {
        return await this._analogItemCollection.Find(_ => true).ToListAsync();
    }

    public async Task<(double rate, double lastReading)> GetBulkH2Usage(int days, bool includeWeekends = true) {
        return await this._usageService.GetBulkH2Usage(days, includeWeekends);
    }
}