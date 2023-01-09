using MonitoringSystem.Shared.Data.EntityDtos;
namespace MonitoringSystem.ConfigApi.EventContracts.Events; 

public class AnalogLevelUpdatedEvent {
    public AnalogLevelDto AnalogLevelDto { get; set; } = default!;
}

public class DiscreteLevelUpdatedEvent {
    public DiscreteLevelDto DiscreteLevelDto { get; set; } = default!;
}