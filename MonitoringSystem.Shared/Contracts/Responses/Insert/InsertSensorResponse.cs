using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Responses.Insert; 

public class InsertSensorResponse {
    public SensorDto Sensor { get; set; } = default!;
}