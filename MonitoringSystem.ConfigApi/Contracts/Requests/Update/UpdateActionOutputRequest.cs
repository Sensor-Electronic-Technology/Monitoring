using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Requests.Update; 

public class UpdateActionOutputRequest {
    public ActionOutputDto ActionOutput { get; set; }
}