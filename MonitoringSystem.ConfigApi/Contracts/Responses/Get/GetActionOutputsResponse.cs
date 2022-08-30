using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Responses.Get; 

public class GetActionOutputsResponse {
    public IEnumerable<ActionOutputDto> ActionOutputs { get; set; } = Enumerable.Empty<ActionOutputDto>();
}