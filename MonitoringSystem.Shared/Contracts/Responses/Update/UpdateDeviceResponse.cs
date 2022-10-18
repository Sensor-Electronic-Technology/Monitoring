using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Responses.Update; 

public class UpdateDeviceResponse {
    public ModbusDeviceDto ModbusDevice { get; set; } = default!;
}