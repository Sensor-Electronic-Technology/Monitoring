using MongoDB.Driver;
using MonitoringSystem.Shared.Data.LogModel;

namespace MonitoringSystem.Shared.Data; 


public class LogRepository {
    private readonly IMongoClient _client;
    private readonly IMongoCollection<AnalogItem> _analogCollection;
    private readonly IMongoCollection<SensorType> _sensorCollection;
    
    public LogRepository(IMongoClient client,string databaseName) {
        var deviceDB = client.GetDatabase(databaseName);
        
    }

}