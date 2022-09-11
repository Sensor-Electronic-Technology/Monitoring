using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Responses.Get;

public class GetAlertResponse {
    public AlertDto Alert { get; set; } = default!;
}

public class GetAnalogAlertResponse {
    public AnalogAlertDto AnalogAlert { get; set; } = default!;
}

public class GetDiscreteAlertResponse {
    public DiscreteAlertDto DiscreteAlert { get; set; } = default!;
}

public class GetAnalogLevelsResponse {
    public IEnumerable<AnalogLevelDto> AnalogLevels { get; set; } = Enumerable.Empty<AnalogLevelDto>();
}

public class GetDiscreteLevelResponse {
    public DiscreteLevelDto DiscreteLevel { get; set; } = default!;
}