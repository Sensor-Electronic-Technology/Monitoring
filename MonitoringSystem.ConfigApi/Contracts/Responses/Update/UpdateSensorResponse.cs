using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Responses.Update; 

public class UpdateSensorResponse {
    public SensorDto Sensor { get; set; } = default!;
}