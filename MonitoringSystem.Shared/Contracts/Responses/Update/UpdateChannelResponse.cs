using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Responses.Update;

public class UpdateAnalogChannelResponse {
    public AnalogInputDto AnalogChannel { get; set; } = default!;
}

public class UpdateDiscreteChannelResponse {
    public DiscreteInputDto DiscreteChannel { get; set; } = default!;
}

public class UpdateVirtualChannelResponse {
    public VirtualInputDto VirtualChannel { get; set; } = default!;
}

public class UpdateOutputChannelResponse {
    public DiscreteOutputDto OutputChannel { get; set; } = default!;
}