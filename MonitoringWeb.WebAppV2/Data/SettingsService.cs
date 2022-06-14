using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;

namespace MonitoringWeb.WebAppV2.Data;

public class SettingsService {
    private WebsiteSettings _settings;
    private IMongoCollection<ManagedDevice> _deviceCollection;
    private List<ManagedDevice> _devices;

    public SettingsService(IOptions<WebsiteSettings> options,IMongoClient client) {
        this._settings = options.Value;
        var database = client.GetDatabase(this._settings.DatabaseName);
        this._deviceCollection = database.GetCollection<ManagedDevice>(this._settings.CollectionName);

    }

    public Task<IEnumerable<ManagedDevice>> GetDevices() {
        return Task.FromResult(this._devices.AsEnumerable());
    }

    public Task<IEnumerable<string>> GetHubAddresses() {
        return Task.FromResult(this._devices.Select(e => e.HubAddress));
    }

    public async Task Load() {
        this._devices=await this._deviceCollection.Find(_ => true).ToListAsync();
    }
}

