using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringWeb.WebApp.Data;

namespace MonitoringWeb.WebApp.Services; 

public class BulkGasProvider {
    private readonly IMongoClient _client;
    private IMongoCollection<WebsiteBulkSettings> _settingsCollection;
    private readonly IMongoCollection<BulkEmailSettings> _emailSettingsCollection;

    public BulkGasProvider(IMongoClient client,IOptions<MonitorWebsiteSettings> options) {
        this._client = client;
        var database = this._client.GetDatabase(options.Value.DatabaseName);
        this._settingsCollection = database.GetCollection<WebsiteBulkSettings>(options.Value.BulkSettingsCollection);
        this._emailSettingsCollection = database.GetCollection<BulkEmailSettings>(options.Value.BulkEmailSettingsCollection);
    }

    public async Task Update(WebsiteBulkSettings settings) {
        var update=Builders<WebsiteBulkSettings>.Update
            .Set(e => e.H2Settings, settings.H2Settings)
            .Set(e => e.N2Settings, settings.N2Settings)
            .Set(e => e.NHSettings, settings.NHSettings)
            .Set(e => e.RefreshTime, settings.RefreshTime);
        await this._settingsCollection.UpdateOneAsync(e => e._id == settings._id, update);
    }

    public async Task<WebsiteBulkSettings> GetSettings() {
        return await this._settingsCollection.Find(_ => true)
            .FirstOrDefaultAsync();
    }
    
    public async Task UpdateEmailSettings(BulkEmailSettings settings) {
        var update = Builders<BulkEmailSettings>.Update
            .Set(e => e.ToAddresses, settings.ToAddresses)
            .Set(e=>e.CcAddresses,settings.CcAddresses);
        await this._emailSettingsCollection.UpdateOneAsync(e => e._id == settings._id, update);
    }

    public async Task<BulkEmailSettings> GetEmailSettings() {
        return await this._emailSettingsCollection.Find(_ => true)
            .FirstOrDefaultAsync();
    }
}