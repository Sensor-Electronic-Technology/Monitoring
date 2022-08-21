using MongoDB.Bson;

namespace MonitoringData.DataApi.Contracts.Request;

public abstract class UpdateMonitorItemRequest {
    public string DeviceName { get; set; } = default!;
    public ObjectId _id { get; set; }
}

public class UpdateAnalogItemRequest {
    //Add All relevant fields
}

public class UpdateDiscreteItemRequest {
    //Add All relevant fields
}

public class UpdateVirtualItemRequest {
    //Add All relevant fields
}

public class UpdateActionItemRequest {
    //Add All relevant fields
}

public class UpdateMonitorAlertRequest {
    //Add All relevant fields
}