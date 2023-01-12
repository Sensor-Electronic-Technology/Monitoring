using MonitoringConfig.Data.Model;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Mapping; 

public static class ChannelMapping {

    public static AnalogInputDto ToDto(this AnalogInput input) {
        return new AnalogInputDto() {
            Id = input.Id,
            Identifier = input.Identifier,
            DisplayName = input.DisplayName,
            SystemChannel = input.SystemChannel,
            Connected = input.Connected,
            Bypass = input.Bypass,
            Display = input.Display,
            RegisterAddress = input.ModbusAddress?.Address ?? 0,
            RegisterLength=input.ModbusAddress?.RegisterLength ?? 0,
            RegisterType = input.ModbusAddress?.RegisterType ??0,
            ChannelAddress=input.ChannelAddress?.Channel ?? 0,
            ModuleSlot=input.ChannelAddress?.ModuleSlot ?? 0,
            SensorId = input.SensorId,
            ModbusDeviceId = input.ModbusDeviceId
        };
    }

    public static AnalogInput ToEntity(this AnalogInputDto input) {
        var entity=new AnalogInput() {
            Id = input.Id,
            Identifier = input.Identifier,
            DisplayName = input.DisplayName,
            SystemChannel = input.SystemChannel,
            Connected = input.Connected,
            Bypass = input.Bypass,
            Display = input.Display,
            ModbusAddress = new ModbusAddress() {
                Address=input.RegisterAddress,
                RegisterLength=input.RegisterLength,
                RegisterType=input.RegisterType
            },
            ChannelAddress=new ChannelAddress() {
                Channel=input.ChannelAddress,
                ModuleSlot=input.ModuleSlot
            },
            SensorId = input.SensorId,
            ModbusDeviceId = input.ModbusDeviceId,
        };

        return entity;
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
            RegisterAddress = input.ModbusAddress.Address,
            RegisterLength=input.ModbusAddress.RegisterLength,
            RegisterType = input.ModbusAddress.RegisterType,
            ChannelAddress=input.ChannelAddress.Channel,
            ModuleSlot=input.ChannelAddress.ModuleSlot,
            ModbusDeviceId = input.ModbusDeviceId
        };
        return dto;
    }
    
    public static DiscreteInput ToEntity(this DiscreteInputDto input) {
        var entity=new DiscreteInput() {
            Id = input.Id,
            Identifier = input.Identifier,
            DisplayName = input.DisplayName,
            SystemChannel = input.SystemChannel,
            Connected = input.Connected,
            Bypass = input.Bypass,
            Display = input.Display,
            ModbusAddress = new ModbusAddress() {
                Address=input.RegisterAddress,
                RegisterLength=input.RegisterLength,
                RegisterType=input.RegisterType
            },
            ChannelAddress=new ChannelAddress() {
                Channel=input.ChannelAddress,
                ModuleSlot=input.ModuleSlot
            },
            ModbusDeviceId = input.ModbusDeviceId
        };
        return entity;
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
            RegisterAddress = input.ModbusAddress.Address,
            RegisterLength=input.ModbusAddress.RegisterLength,
            RegisterType = input.ModbusAddress.RegisterType,
            ChannelAddress=input.ChannelAddress.Channel,
            ModuleSlot=input.ChannelAddress.ModuleSlot,
            StartState=input.StartState,
            ModbusDeviceId = input.ModbusDeviceId
        };
    }
    
    public static DiscreteOutput ToEntity(this DiscreteOutputDto input) {
        var entity=new DiscreteOutput() {
            Id = input.Id,
            Identifier = input.Identifier,
            DisplayName = input.DisplayName,
            SystemChannel = input.SystemChannel,
            Connected = input.Connected,
            Bypass = input.Bypass,
            Display = input.Display,
            ModbusAddress = new ModbusAddress() {
                Address=input.RegisterAddress,
                RegisterLength=input.RegisterLength,
                RegisterType=input.RegisterType
            },
            ChannelAddress=new ChannelAddress() {
                Channel=input.ChannelAddress,
                ModuleSlot=input.ModuleSlot
            },
            ModbusDeviceId = input.ModbusDeviceId
        };
        return entity;
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
            RegisterAddress = input.ModbusAddress.Address,
            RegisterLength=input.ModbusAddress.RegisterLength,
            RegisterType = input.ModbusAddress.RegisterType,
            ChannelAddress=input.ChannelAddress.Channel,
            ModuleSlot=input.ChannelAddress.ModuleSlot,
            ModbusDeviceId = input.ModbusDeviceId
        };
        return dto;
    }
    
    public static VirtualInput ToEntity(this VirtualInputDto input) {
        var entity=new VirtualInput() {
            Id = input.Id,
            Identifier = input.Identifier,
            DisplayName = input.DisplayName,
            SystemChannel = input.SystemChannel,
            Connected = input.Connected,
            Bypass = input.Bypass,
            Display = input.Display,
            ModbusAddress = new ModbusAddress() {
                Address=input.RegisterAddress,
                RegisterLength=input.RegisterLength,
                RegisterType=input.RegisterType
            },
            ChannelAddress=new ChannelAddress() {
                Channel=input.ChannelAddress,
                ModuleSlot=input.ModuleSlot
            },
            ModbusDeviceId = input.ModbusDeviceId
        };
        return entity;
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
            ThresholdInterval = sensor.ThresholdInterval,
            ValueDirection = sensor.ValueDirection,
            Units = sensor.Units,
            YAxisMax = sensor.YAxitMax,
            YAxisMin = sensor.YAxisMin
        };
    }
    
    public static Sensor ToEntity(this SensorDto sensor) {
        return new Sensor() {
            Id = sensor.Id,
            DisplayName = sensor.DisplayName,
            Description = sensor.Description,
            Factor = sensor.Factor,
            Name = sensor.Name,
            Offset = sensor.Offset,
            Slope = sensor.Slope,
            RecordThreshold = sensor.RecordThreshold,
            ThresholdInterval = sensor.ThresholdInterval,
            ValueDirection = sensor.ValueDirection,
            Units = sensor.Units,
            YAxitMax = sensor.YAxisMax,
            YAxisMin = sensor.YAxisMin
        };
    }
}