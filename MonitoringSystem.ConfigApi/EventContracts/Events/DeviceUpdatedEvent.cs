using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.EventContracts.Events; 

public class DeviceUpdatedEvent {
    public ModbusDeviceDto ModbusDevice { get; set; } = default!;
}