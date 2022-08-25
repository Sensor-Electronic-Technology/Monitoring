namespace MonitoringSystem.ConfigApi.Contracts.Requests.Get; 

public class GetDeviceActionsRequest {
    public Guid DeviceId { get; set; }
}

public class GetDeviceActionRequest {
    public Guid DeviceActionId { get; set; }
}