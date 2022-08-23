using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Responses.Get; 

public class GetDeviceChannelsResponse {
    public IEnumerable<AnalogInputDto> AnalogInputs { get; set; } = Enumerable.Empty<AnalogInputDto>();
    public IEnumerable<DiscreteInputDto> DiscreteInputs { get; set; }= Enumerable.Empty<DiscreteInputDto>();
    public IEnumerable<VirtualInputDto> VirtualInputs { get; set; }= Enumerable.Empty<VirtualInputDto>();
    public IEnumerable<DiscreteOutputDto> DiscreteOutputs { get; set; }= Enumerable.Empty<DiscreteOutputDto>();
}
