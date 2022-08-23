using MonitoringConfig.Data.Model;

namespace MonitoringSystem.ConfigApi.Contracts.Responses.Get;

public class GetAnalogChannelAlertResponse {
    public AnalogAlert AnalogAlert { get; set; } = default!;
}

public class GetDiscreteChannelAlertResponse {
    public DiscreteAlert DiscreteAlert { get; set; } = default!;
}

public class GetVirtualChannelAlertResponse {
    public DiscreteAlert DiscreteAlert { get; set; } = default!;
}

