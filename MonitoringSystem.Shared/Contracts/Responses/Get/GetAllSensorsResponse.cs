using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Responses.Get; 

public class GetAllSensorsResponse {
    public IEnumerable<SensorDto> Sensors { get; set; } = default!;
}