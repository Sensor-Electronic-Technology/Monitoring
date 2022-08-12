using MongoDB.Bson;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Data;
using MonitoringSystem.Shared.Data;

namespace MonitoringWeb.TestConfig.Data;

public class DiscreteChannelDto {
    public ObjectId _id { get; set; }
    public string ChannelName { get; set; }
    public string DisplayName { get; set; }
    public bool Display { get; set; }
    public ModbusAddress ModbusAddress { get; set; }
    public int SystemChannel { get; set; }
    public ModuleAddress ChannelAddress { get; set; }
    public bool Connected { get; set; }
    public MonitorDiscreteAlert Alert { get; set; }
}

public class DiscreteChannelDataService {
    private readonly IMongoCollection<ModuleDiscreteChannel> _channelCollection;
    private readonly IMongoCollection<MonitorDiscreteAlert> _alertCollection;
    private readonly IMongoCollection<MonitorAction> _actionCollection;
    private ILogger<DiscreteChannelDataService> _logger;

    public DiscreteChannelDataService(ILogger<DiscreteChannelDataService> logger) {
        var client = new MongoClient("mongodb://172.20.3.41");
        var deviceDatabase = client.GetDatabase("epi1_data_dev");
        this._channelCollection = deviceDatabase.GetCollection<ModuleDiscreteChannel>("discrete_items");
        this._alertCollection = deviceDatabase.GetCollection<MonitorDiscreteAlert>("alert_items");
        this._actionCollection = deviceDatabase.GetCollection<MonitorAction>("action_items");
        this._logger = logger;
    }

    public async Task<IEnumerable<ModuleDiscreteChannel>> GetDiscreteChannels() {

        return null;
    }



}