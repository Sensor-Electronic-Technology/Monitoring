using MonitoringSystem.Shared.Data.EntityDtos;
namespace MonitoringSystem.Shared.Contracts.Requests.Insert; 

public class InsertSensorRequest {
    public SensorDto Sensor { get; set; } = default!;
}