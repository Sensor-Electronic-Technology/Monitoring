namespace MonitoringData.DataApi.Contracts.Request; 

public class GetAllAnalogItemsRequest {
    public string DeviceName { get; set; } = default!;
}