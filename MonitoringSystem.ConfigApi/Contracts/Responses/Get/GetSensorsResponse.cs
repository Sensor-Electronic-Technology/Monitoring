using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Responses.Get; 

public class GetSensorsResponse {
    public IEnumerable<SensorDto> Sensors { get; set; } = default!;
}