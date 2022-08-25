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
            Register = alert.ModbusAddress.Address,
            RegisterLength = alert.ModbusAddress.RegisterLength,
            RegisterType=alert.ModbusAddress.RegisterType,
            Name = alert.Name,
            InputChannelId = alert.InputChannelId
        };
    }
    public static DiscreteAlertDto ToDto(this DiscreteAlert alert) {
        var dto=new DiscreteAlertDto() {
            Id = alert.Id,
            Bypass = alert.Bypass,
            BypassResetTime = alert.BypassResetTime,
            Enabled = alert.Enabled,
            Register = alert.ModbusAddress.Address,
            RegisterLength = alert.ModbusAddress.RegisterLength,
            RegisterType=alert.ModbusAddress.RegisterType,
            Name = alert.Name,
            InputChannelId = alert.InputChannelId,
        };
        return dto;
    }
    public static AnalogLevelDto ToDto(this AnalogLevel level) {
        var dto=new AnalogLevelDto() {
            Id=level.Id,
            Bypass=level.Bypass,
            BypassResetTime = level.BypassResetTime,
            Enabled=level.Enabled,
            SetPoint = level.SetPoint,
            DeviceActionId = level.DeviceActionId,
            AnalogAlertId = level.AnalogAlertId
        };
        return dto;
    }
    public static DiscreteLevelDto ToDto(this DiscreteLevel level) {
        var dto=new DiscreteLevelDto() {
            Id=level.Id,
            Bypass=level.Bypass,
            BypassResetTime = level.BypassResetTime,
            Enabled=level.Enabled,
            TriggerOn = level.TriggerOn,
            DeviceActionId = level.DeviceActionId,
            DiscreteAlertId = level.DiscreteAlertId
        };
        return dto;
    }
    public static AnalogAlert ToEntity(this AnalogAlertDto alert) {
        return new AnalogAlert() {
            Id = alert.Id,
            Bypass = alert.Bypass,
            BypassResetTime = alert.BypassResetTime,
            Enabled = alert.Enabled,
            ModbusAddress=new ModbusAddress() {
                Address=alert.Register,
                RegisterLength=alert.RegisterLength,
                RegisterType=alert.RegisterType
            },
            Name = alert.Name,
            InputChannelId = alert.InputChannelId
        };
    }
    public static DiscreteAlert ToEntity(this DiscreteAlertDto alert) {
        return new DiscreteAlert() {
            Id = alert.Id,
            Bypass = alert.Bypass,
            BypassResetTime = alert.BypassResetTime,
            Enabled = alert.Enabled,
            ModbusAddress=new ModbusAddress() {
                Address=alert.Register,
                RegisterLength=alert.RegisterLength,
                RegisterType=alert.RegisterType
            },
            Name = alert.Name,
            InputChannelId = alert.InputChannelId
        };
    }
    public static AnalogLevel ToEntity(this AnalogLevelDto level) {
        var entity=new AnalogLevel() {
            Id=level.Id,
            Bypass=level.Bypass,
            BypassResetTime = level.BypassResetTime,
            Enabled=level.Enabled,
            SetPoint = level.SetPoint,
            AnalogAlertId = level.AnalogAlertId,
            DeviceActionId = level.DeviceActionId
        };
        return entity;
    }
    public static DiscreteLevel ToEntity(this DiscreteLevelDto level) {
        var entity=new DiscreteLevel() {
            Id=level.Id,
            Bypass=level.Bypass,
            BypassResetTime = level.BypassResetTime,
            Enabled=level.Enabled,
            TriggerOn = level.TriggerOn,
            DeviceActionId = level.DeviceActionId,
            DiscreteAlertId = level.DiscreteAlertId
        };
        return entity;
    }
}