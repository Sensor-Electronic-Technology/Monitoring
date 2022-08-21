using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.LogModel;
public class MonitorItem {
    public ObjectId _id { get; set; }
    public string ItemId { get; set; }
    public string Identifier { get; set; }
    public bool Display { get; set; }
    public ObjectId ManagedDeviceId { get; set; }
}

public class MonitorChannel : MonitorItem {
    public int SystemChannel { get; set; }
}

public class AnalogItem : MonitorChannel {
    public ObjectId SensorId { get; set; }
    public float RecordThreshold { get; set; }
    public int Factor { get; set; }
    public int Register { get; set; }
    public int RegisterLength { get; set; }
    public ActionType Level1Action { get; set; }
    public float Level1SetPoint { get; set; }
    public ActionType Level2Action { get; set; }
    public float Level2SetPoint { get; set; }
    public ActionType Level3Action { get; set; }
    public float Level3SetPoint { get; set; }
}

public class DiscreteItem : MonitorChannel {
    public ActionType ActionType { get; set; }
    public DiscreteState TriggerOn { get; set; }
}

public class OutputItem : MonitorChannel {

}

public class VirtualItem : MonitorChannel {
    public int Register { get; set; }
}

public class ActionItem : MonitorItem {
    public ActionType ActionType { get; set; }
    public bool EmailEnabled { get; set; }
    public int EmailPeriod { get; set; }
}
