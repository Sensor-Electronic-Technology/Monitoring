using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Responses.Update;

public class UpdateAlertResponse {
    public AlertDto Alert { get; set; } = default!;
}

public class UpdateAnalogAlertResponse {
    public AnalogAlertDto AnalogAlert { get; set; } = default!;
}

public class UpdateDiscreteAlertResponse {
    public DiscreteAlertDto DiscreteAlert { get; set; } = default!;
}