using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.UsageModel;

namespace MonitoringData.Infrastructure.Services; 

public class UsageService {
    private readonly ILogger<UsageService> _logger;
    private IMongoCollection<AnalogItem> _analogCollection;
    private IMongoCollection<UsageDayRecord> _usageCollection;
    private readonly DataLogConfigProvider _configProvider;
    private readonly IMongoClient _client;

    public UsageService(IMongoClient client, ILogger<UsageService> logger,
            DataLogConfigProvider provider) {
        this._logger = logger;
        this._configProvider = provider;
        this._client = client;
    }

    /*public async Task<IEnumerable<DayRecord>> GetUsageData(string database,string channel) {
        
    }*/

}