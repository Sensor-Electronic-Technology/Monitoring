using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Responses.Get; 

public class GetActionOutputsResponse {
    public IEnumerable<ActionOutputDto> ActionOutputs { get; set; } = Enumerable.Empty<ActionOutputDto>();
}