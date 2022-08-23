using MonitoringConfig.Data.Model;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Mapping;

public static class AlertMapping {
    public static AnalogAlertDto ToDto(this AnalogAlert alert) {
        return new AnalogAlertDto() {
            Id = alert.Id,
            Bypass = alert.Bypass,
            BypassResetTime = alert.BypassResetTime,
            Enabled = alert.Enabled,
            ModbusAddress = alert.ModbusAddress,
            Name = alert.Name,
            AnalogLevels = alert.AlertLevels.Select(e=>e.ToDto())
        };
    }

    public static DiscreteAlertDto ToDto(this DiscreteAlert alert) {
        var dto=new DiscreteAlertDto() {
            Id = alert.Id,
            Bypass = alert.Bypass,
            BypassResetTime = alert.BypassResetTime,
            Enabled = alert.Enabled,
            ModbusAddress = alert.ModbusAddress,
            Name = alert.Name
        };
        if (alert.AlertLevel is not null) {
            dto.DiscreteLevelDto = alert.AlertLevel.ToDto();
        }
        return dto;
    }

    public static AnalogLevelDto ToDto(this AnalogLevel level) {
        var dto=new AnalogLevelDto() {
            Id=level.Id,
            Bypass=level.Bypass,
            BypassResetTime = level.BypassResetTime,
            Enabled=level.Enabled,
            SetPoint = level.SetPoint
        };
        if (level.DeviceAction is not null) {
            dto.DeviceAction = level.DeviceAction.ToDto();
        }
        return dto;
    }
    
    public static DiscreteLevelDto ToDto(this DiscreteLevel level) {
        var dto=new DiscreteLevelDto() {
            Id=level.Id,
            Bypass=level.Bypass,
            BypassResetTime = level.BypassResetTime,
            Enabled=level.Enabled,
            TriggerOn = level.TriggerOn
        };
        if (level.DeviceAction is not null) {
            dto.DeviceAction = level.DeviceAction.ToDto();
        }
        return dto;
    }
    
}