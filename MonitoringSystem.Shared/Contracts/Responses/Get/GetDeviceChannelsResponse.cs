using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Responses.Get; 

public class GetAnalogChannelsResponse {
    public IEnumerable<AnalogInputDto> AnalogInputs { get; set; } = Enumerable.Empty<AnalogInputDto>();
}

public class GetDiscreteChannelsResponse {
    public IEnumerable<DiscreteInputDto> DiscreteInputs { get; set; }= Enumerable.Empty<DiscreteInputDto>();
}

public class GetVirtualChannelsResponse {
    public IEnumerable<VirtualInputDto> VirtualInputs { get; set; } = Enumerable.Empty<VirtualInputDto>();
}

public class GetOutputChannelsResponse {
    public IEnumerable<DiscreteOutputDto> OutputChannels { get; set; } = Enumerable.Empty<DiscreteOutputDto>();
}
