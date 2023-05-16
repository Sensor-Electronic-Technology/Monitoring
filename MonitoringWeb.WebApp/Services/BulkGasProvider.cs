using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringWeb.WebApp.Data;

namespace MonitoringWeb.WebApp.Services; 

public class BulkGasProvider {
    private readonly IMongoClient _client;
    private IMongoCollection<WebsiteBulkSettings> _settingsCollection;

    public BulkGasProvider(IMongoClient client,IOptions<MonitorWebsiteSettings> options) {
        this._client = client;
        var database = this._client.GetDatabase(options.Value.DatabaseName);
        this._settingsCollection = database.GetCollection<WebsiteBulkSettings>(options.Value.BulkSettingsCollection);
    }

    public async Task Update(WebsiteBulkSettings settings) {
        var update=Builders<WebsiteBulkSettings>.Update
            .Set(e => e.H2Settings, settings.H2Settings)
            .Set(e => e.N2Settings, settings.N2Settings)
            .Set(e => e.RefreshTime, settings.RefreshTime);
        await this._settingsCollection.UpdateOneAsync(e => e._id == settings._id, update);
    }

    public async Task<WebsiteBulkSettings> GetSettings() {
        return await this._settingsCollection.Find(_ => true)
            .FirstOrDefaultAsync();
    }
}