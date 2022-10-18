using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Requests.Update; 

public class UpdateSensorRequest {
    public SensorDto Sensor { get; set; } = default!;
}