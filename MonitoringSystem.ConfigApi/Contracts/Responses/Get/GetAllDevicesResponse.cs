using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Responses.Get; 

public class GetAllDevicesResponse {
    public IEnumerable<ModbusDeviceDto> Devices { get; set; } = Enumerable.Empty<ModbusDeviceDto>();
}