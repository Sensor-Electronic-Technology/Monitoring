using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.LogModel;

public class SensorType {
    public ObjectId _id { get; set; }
    public string Name { get; set; }
    public string Units { get; set; }
    public double Slope { get; set; }
    public double Offset { get; set; }
    public int YAxisStart { get; set; }
    public int YAxisStop { get; set; }
}

public class SensorTypeDev {
    public ObjectId _id { get; set; }
    public string Name { get; set; }
    public string Units { get; set; }
    public double Slope { get; set; }
    public double Offset { get; set; }
    public int YAxisStart { get; set; }
    public int YAxisStop { get; set; }
}