

using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Responses.Get;

public class GetAnalogChannelAlertResponse {
    public AnalogAlertDto AnalogAlert { get; set; } = default!;
}

public class GetDiscreteChannelAlertResponse {
    public DiscreteAlertDto DiscreteAlert { get; set; } = default!;
}

public class GetVirtualChannelAlertResponse {
    public DiscreteAlertDto DiscreteAlert { get; set; } = default!;
}

