using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Responses.Update; 

public class UpdateActionOutputResponse {
    public ActionOutputDto? ActionOutput { get; set; } = default!;
}