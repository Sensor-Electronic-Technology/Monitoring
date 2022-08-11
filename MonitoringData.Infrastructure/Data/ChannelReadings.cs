using MongoDB.Bson;
using MonitoringSystem.Shared.Data;
namespace MonitoringData.Infrastructure.Data; 

public class MonitorReading {
    public ObjectId ItemId { get; set; }
}

public class AnalogReading : MonitorReading {
    public double Value { get; set; }
}

public class DiscreteReading : MonitorReading {
    public bool Value { get; set; }
}

public class VirtualReading : MonitorReading {
    public bool Value { get; set; }
}

public class AlertReading : MonitorReading {
    public ActionType state { get; set; }
    public float reading { get; set; }
}

public class ItemReadings {
    public ObjectId _id { get; set; }
    public DateTime Timestamp { get; set; }
}

public class AnalogReadings:ItemReadings {
    public AnalogReading[] Readings { get; set; }    }

public class AlertReadings:ItemReadings {
    public AlertReading[] Readings { get; set; }
}

public class DiscreteReadings : ItemReadings {
    public DiscreteReading[] Readings { get; set; }
}

public class VirtualReadings : ItemReadings {
    public VirtualReading[] Readings { get; set; }
}