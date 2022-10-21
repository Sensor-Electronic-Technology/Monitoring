using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.EventContracts.Events; 

public class AlertUpdatedEvent {
    public AlertDto Alert { get; set; } = default!;
}