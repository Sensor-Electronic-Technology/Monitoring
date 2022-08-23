using MonitoringConfig.Data.Model;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Mapping; 

public static class ChannelMapping {

    public static AnalogInputDto ToDto(this AnalogInput input) {
        var dto=new AnalogInputDto() {
            Id = input.Id,
            Identifier = input.Identifier,
            DisplayName = input.DisplayName,
            SystemChannel = input.SystemChannel,
            Connected = input.Connected,
            Bypass = input.Bypass,
            Display = input.Display,
            ModbusAddress = input.ModbusAddress,
            ChannelAddress = input.ChannelAddress
        };
        if (input.Alert is AnalogAlert alert) {
            dto.Alert = alert.ToDto();
        }
        
        if (input.Sensor is not null) {
            dto.Sensor = input.Sensor.ToDto();
        }
        
        return dto;
    }
    
    public static DiscreteInputDto ToDto(this DiscreteInput input) {
        var dto=new DiscreteInputDto() {
            Id = input.Id,
            Identifier = input.Identifier,
            DisplayName = input.DisplayName,
            SystemChannel = input.SystemChannel,
            Connected = input.Connected,
            Bypass = input.Bypass,
            Display = input.Display,
            ModbusAddress = input.ModbusAddress,
            ChannelAddress = input.ChannelAddress
        };
        if (input.Alert is DiscreteAlert alert) {
            dto.Alert = alert.ToDto();
        }
        return dto;
    }
    
    public static DiscreteOutputDto ToDto(this DiscreteOutput input) {
        return new DiscreteOutputDto() {
            Id = input.Id,
            Identifier = input.Identifier,
            DisplayName = input.DisplayName,
            SystemChannel = input.SystemChannel,
            Connected = input.Connected,
            Bypass = input.Bypass,
            Display = input.Display,
            ModbusAddress = input.ModbusAddress,
            ChannelAddress = input.ChannelAddress,
            StartState=input.StartState
        };
    }
    
    public static VirtualInputDto ToDto(this VirtualInput input) {
        var dto=new VirtualInputDto() {
            Id = input.Id,
            Identifier = input.Identifier,
            DisplayName = input.DisplayName,
            SystemChannel = input.SystemChannel,
            Connected = input.Connected,
            Bypass = input.Bypass,
            Display = input.Display,
            ModbusAddress = input.ModbusAddress,
            ChannelAddress = input.ChannelAddress
        };
        if (input.Alert is DiscreteAlert alert) {
            dto.Alert = alert.ToDto();
        }
        return dto;
    }

    public static SensorDto ToDto(this Sensor sensor) {
        return new SensorDto() {
            Id = sensor.Id,
            DisplayName = sensor.DisplayName,
            Description = sensor.Description,
            Factor = sensor.Factor,
            Name = sensor.Name,
            Offset = sensor.Offset,
            Slope = sensor.Slope,
            RecordThreshold = sensor.RecordThreshold,
            Units = sensor.Units,
            YAxisMax = sensor.YAxitMax,
            YAxisMin = sensor.YAxisMin
        };
    }
    
}