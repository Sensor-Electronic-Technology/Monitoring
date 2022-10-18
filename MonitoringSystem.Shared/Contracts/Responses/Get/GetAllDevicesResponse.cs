using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Responses.Get; 

public class GetAllDevicesResponse {
    public IEnumerable<ModbusDeviceDto> Devices { get; set; } = Enumerable.Empty<ModbusDeviceDto>();
}