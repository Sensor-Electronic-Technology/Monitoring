using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.LogModel;
public class ItemReading {
    public ObjectId MonitorItemId { get; set; }
}

public class AnalogReading : ItemReading {
    public double Value { get; set; }
}

public class DiscreteReading : ItemReading {
    public bool Value { get; set; }
}

public class OutputReading : ItemReading {
    public bool Value { get; set; }
}

public class VirtualReading : ItemReading {
    public bool Value { get; set; }
}

public class AlertReading : ItemReading {
    public ActionType AlertState { get; set; }
    public float Reading { get; set; }
}
public class ItemReadings {
    public ObjectId _id { get; set; }
    public DateTime timestamp { get; set; }
}



public class AnalogReadings:ItemReadings {
    public AnalogReading[] readings { get; set; }    }

public class AlertReadings:ItemReadings {
    public AlertReading[] readings { get; set; }
}

public class DiscreteReadings : ItemReadings {
    public DiscreteReading[] readings { get; set; }
}

public class VirtualReadings : ItemReadings {
    public VirtualReading[] readings { get; set; }
}
