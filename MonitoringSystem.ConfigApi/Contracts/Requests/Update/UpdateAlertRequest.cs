
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Requests.Update; 

public class UpdateAnalogAlertRequest {
    public AnalogAlertDto AnalogAlert { get; set; } = default!;
}

public class UpdateDiscreteAlertRequest {
    public DiscreteAlertDto DiscreteAlert { get; set; } = default!;
}