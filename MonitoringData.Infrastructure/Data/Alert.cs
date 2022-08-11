using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MonitoringSystem.Shared.Data;
namespace MonitoringData.Infrastructure.Data;

public class MonitorActionOutput {
    public ObjectId DiscreteOutputId { get; set; }
    public DiscreteState OnLevel { get; set; }
    public DiscreteState OffLevel { get; set; }
}

public class MonitorAction {
    public ObjectId _id { get; set; }
    public string Name { get; set; }
    public int FirmwareId { get; set; }
    public bool EmailEnabled { get; set; }
    public int EmailPeriod { get; set; }
    public ActionType ActionType { get; set; }
    public  List<MonitorActionOutput> ActionOutputs { get; set; }
}

public abstract class MonitorAlertLevel {
    public string Name { get; set; }
    public bool Enabled { get; set; }
    public ObjectId FacilityActionId { get; set; }
}

public class MonitorAnalogLevel : MonitorAlertLevel {
    public double SetPoint { get; set; }
}

public class MonitorDiscretLevel : MonitorAlertLevel {
    public DiscreteState TriggerOn { get; set; }
}

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(MonitorAnalogAlert),typeof(MonitorDiscreteAlert))]
public abstract class ChannelAlert {
    public ObjectId _id { get; set; }
    public ObjectId ItemId { get; set; }
    public string Name { get; set; }
    public bool Bypass { get; set; }
    public int BypassResetTime { get; set; }
    public bool Enabled { get; set; }
    public ModbusAddress ModbusAddress { get; set; }
}

public class MonitorAnalogAlert : ChannelAlert {
    public List<MonitorAnalogLevel> AlertLevels { get; set; }
}

public class MonitorDiscreteAlert : ChannelAlert {
    public MonitorDiscretLevel AlertLevel { get; set; }
}