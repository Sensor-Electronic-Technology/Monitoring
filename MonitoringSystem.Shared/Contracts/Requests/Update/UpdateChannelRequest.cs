﻿using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Requests.Update;

public class UpdateAnalogChannelRequest {
    public AnalogInputDto AnalogChannel { get; set; } = default!;
}

public class UpdateDiscreteChannelRequest {
    public DiscreteInputDto DiscreteChannel { get; set; } = default!;
}

public class UpdateVirtualChannelRequest {
    public VirtualInputDto VirtualChannel { get; set; } = default!;
}

public class UpdateOutputChannelRequest {
    public DiscreteOutputDto OutputChannel { get; set; } = default!;
}