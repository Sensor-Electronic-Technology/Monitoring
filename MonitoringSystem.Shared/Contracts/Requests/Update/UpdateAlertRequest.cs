
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Requests.Update;

public class UpdateAlertRequest {
    public AlertDto Alert { get; set; } = default!;
}

public class UpdateAnalogAlertRequest {
    public AnalogAlertDto AnalogAlert { get; set; } = default!;
}

public class UpdateDiscreteAlertRequest {
    public DiscreteAlertDto DiscreteAlert { get; set; } = default!;
}