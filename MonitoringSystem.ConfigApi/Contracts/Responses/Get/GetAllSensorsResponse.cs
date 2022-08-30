using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Responses.Get; 

public class GetAllSensorsResponse {
    public IEnumerable<SensorDto> Sensors { get; set; } = default!;
}