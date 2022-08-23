namespace MonitoringSystem.ConfigApi.Contracts.Requests.Get; 

public abstract class GetChannelAlertRequest {
    public Guid ChannelId { get; set; }
}
public class GetAnalogChannelAlert:GetChannelAlertRequest { }
public class GetDiscreteChannelAlert:GetChannelAlertRequest { }
public class GetVirtualChannelAlert:GetChannelAlertRequest { }