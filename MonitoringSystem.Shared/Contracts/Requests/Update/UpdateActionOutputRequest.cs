using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Requests.Update; 

public class UpdateActionOutputRequest {
    public ActionOutputDto ActionOutput { get; set; }
}