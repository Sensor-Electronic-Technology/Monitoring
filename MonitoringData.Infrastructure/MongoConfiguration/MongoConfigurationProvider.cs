using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.MongoConfiguration;

public class MongoConfigurationProvider:ConfigurationProvider {
    private IMongoCollection<ManagedDevice> _deviceCollection;
    private IMongoCollection<EmailRecipient> _emailCollection;
    
    public MongoConfigurationProvider(string connectionString) {
        
    }

    public override void Load() {
        
    }
}