using MonitoringConfig.Data.Model;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Requests.Update; 

public class UpdateDeviceRequest {
    public ModbusDeviceDto ModbusDevice { get; set; } = default!;
}

