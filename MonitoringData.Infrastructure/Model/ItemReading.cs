using MongoDB.Bson;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Model {
    public class ItemReading {
        public int itemid { get; set; }
        public DateTime timestamp { get; set; }
    }


    public class AnalogReading : ItemReading {
        public double value { get; set; }
    }

    public class DiscreteReading : ItemReading {
        public bool value { get; set; }
    }

    public class OutputReading : ItemReading {
        public bool value { get; set; }
    }

    public class VirtualReading : ItemReading {
        public bool value { get; set; }
    }

    public class ActionReading : ItemReading {
        public bool value { get; set; }
    }

    public class AlertReading : ItemReading {
        public ActionType state { get; set; }
        public float reading { get; set; }
    }

    public class DeviceReading : ItemReading {
        public DeviceState value { get; set; }
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
}
