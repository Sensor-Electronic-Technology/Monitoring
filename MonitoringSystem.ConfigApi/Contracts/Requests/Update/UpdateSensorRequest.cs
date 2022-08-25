using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Requests.Update; 

public class UpdateSensorRequest {
    public SensorDto Sensor { get; set; } = default!;
}