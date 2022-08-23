namespace MonitoringSystem.ConfigApi.Contracts.Requests.Get; 

public  abstract class GetAlertLevelRequest {
    public Guid ChannelId { get; set; }
}

public class GetAnalogAlertLevel:GetAlertLevelRequest { }

public class GetDiscreteAlertLevelRequest : GetAlertLevelRequest { }

public class GetVirtualAlertLevelRequest : GetAlertLevelRequest { }