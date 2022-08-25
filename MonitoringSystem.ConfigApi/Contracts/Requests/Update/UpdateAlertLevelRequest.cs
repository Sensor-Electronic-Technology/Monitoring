using MonitoringSystem.Shared.Data.EntityDtos;
namespace MonitoringSystem.ConfigApi.Contracts.Requests.Update;

public class UpdateAnalogLevelRequest {
    public AnalogLevelDto AnalogLevel { get; set; } = default!;
}

public class UpdateDiscreteLevelRequest {
    public DiscreteLevelDto DiscreteLevel { get; set; } = default!;
}