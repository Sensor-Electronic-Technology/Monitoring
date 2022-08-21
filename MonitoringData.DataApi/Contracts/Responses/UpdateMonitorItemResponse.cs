using MongoDB.Bson;

namespace MonitoringData.DataApi.Contracts.Responses; 

public abstract class MonitorItemResponse{
    public ObjectId _id { get; set; }
}

public class UpdateAnalogItemResponse:MonitorItemResponse {
    
}
public class UpdateDiscreteItemResponse:MonitorItemResponse {
    
}
public class UpdateVirtualItemResponse:MonitorItemResponse {
    
}
public class UpdateActionItemResponse:MonitorItemResponse {
    
}