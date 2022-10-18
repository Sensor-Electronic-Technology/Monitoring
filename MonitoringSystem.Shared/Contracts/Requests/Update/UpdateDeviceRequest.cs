using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Requests.Update; 

public class UpdateDeviceRequest {
    public ModbusDeviceDto ModbusDevice { get; set; } = default!;
}

