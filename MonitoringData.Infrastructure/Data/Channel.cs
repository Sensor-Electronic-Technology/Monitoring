using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Data;

public class ModuleAddress {
    public int Channel { get; set; }
    public int ModuleSlot { get; set; }

    public ModuleAddress() {
        this.Channel = 0;
        this.ModuleSlot = 0;
    }

    public ModuleAddress(int channel, int moduleSlot) {
        this.Channel = channel;
        this.ModuleSlot = moduleSlot;
    }
}

public abstract class MonitorChannel {
    public ObjectId _id { get; set; }
    public string ChannelName { get; set; }
    public string DisplayName { get; set; }
    public bool Display { get; set; }
    public ModbusAddress ModbusAddress { get; set; }
}

public abstract class ModuleChannel : MonitorChannel {
    public int SystemChannel { get; set; }
    public ModuleAddress ChannelAddress { get; set; }
    public bool Connected { get; set; }
}

public class ModuleVirtualChannel : MonitorChannel {
    public ObjectId AlertId { get; set; }
}

public class ModuleAnalogChannel:ModuleChannel {
    public ObjectId SensorId { get; set; }
    public ObjectId AlertId { get; set; }
    public int Factor { get; set; }
}

public class ModuleDiscreteChannel : ModuleChannel {
    public ObjectId AlertId { get; set; }
}

public class ModuleOutputChannel:ModuleChannel {
    public DiscreteState StartState { get; set; }
}
