using MonitoringSystem.Shared.Data.LogModel;

namespace MonitoringData.DataApi.Contracts.Responses; 

public class GetAllSensorTypesResponse {
    public IEnumerable<SensorType> SensorTypes { get; set; } = Enumerable.Empty<SensorType>();
}