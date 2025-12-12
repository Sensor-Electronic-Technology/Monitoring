using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;
using MonitoringSystem.Shared.Services;
using MonitoringWeb.WebApp.Components.Bulkgas.BulkH2;

namespace MonitoringWeb.WebApp.Services;

public class BulkH2CalcService {
    private readonly IMongoCollection<AnalogItem> _analogItemCollection;
    private readonly IMongoCollection<BulkH2CalcSettings> _bulkH2CalcSettingsCollection;
    private readonly UsageService _usageService;
    
    public BulkH2CalcService(IMongoClient client,UsageService usageService,IOptions<MonitorWebsiteSettings> options) {
        var epiDatabase = client.GetDatabase("epi1_data");
        this._analogItemCollection = epiDatabase.GetCollection<AnalogItem>("analog_items");
        var database = client.GetDatabase(options.Value.DatabaseName);
        this._bulkH2CalcSettingsCollection = database.GetCollection<BulkH2CalcSettings>(options.Value.BulkH2CalcSettingsCollection);
        this._usageService = usageService;
    }
    
    public async Task<BulkH2SettingsDto> GetSettings() {
        var bulkSettings=await this._bulkH2CalcSettingsCollection.Find(_ => true).FirstOrDefaultAsync();
        var anlogItem=await this._analogItemCollection.Find(e => e.Identifier == "Bulk H2(PSI)").FirstOrDefaultAsync();
        return new BulkH2SettingsDto() {
            AnalogItem = anlogItem,
            BulkH2CalcSettings = bulkSettings
        };
    }
    
    public async Task UpdateSettings(BulkH2SettingsDto settings) {
        if(settings.BulkH2CalcSettings==null) return;
        
        var update=Builders<BulkH2CalcSettings>.Update
            .Set(e=>e.DaysFromAlarm,settings.BulkH2CalcSettings.DaysFromAlarm)
            .Set(e=>e.IncludeWeekends,settings.BulkH2CalcSettings.IncludeWeekends)
            .Set(e=>e.MinSoftWarnLevel,settings.BulkH2CalcSettings.MinSoftWarnLevel)
            .Set(e=>e.TimeStamp,DateTime.UtcNow);
        await this._bulkH2CalcSettingsCollection.UpdateOneAsync(e=>e._id==settings.BulkH2CalcSettings._id,update);
    }
    
    public async Task<List<AnalogItem>> GetAnalogItems() {
        return await this._analogItemCollection.Find(_ => true).ToListAsync();
    }
    
    public async Task<(double rate,double lastReading)> GetBulkH2Usage(int days,bool includeWeekends = true) {
        return await this._usageService.GetBulkH2Usage(days,includeWeekends);
    }
    

    
}