using FastEndpoints;
using MonitoringConfig.Data.Model;
using MonitoringSystem.Shared.Contracts.Responses.Get;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Mapping; 

public static class DeviceMapper {

    public static GetAllDevicesResponse ToDeviceResponse(this List<ModbusDevice> devices) {
        return new GetAllDevicesResponse() {
            Devices = devices.Select(e => e.ToDto())
        };
    }

    public static ModbusDevice FromUpdateRequest(this ModbusDeviceDto device) {
        return device.ToEntity();
    }
    
    public static ModbusDeviceDto ToDto(this ModbusDevice device) {
        var modbusDevice=new ModbusDeviceDto() {
            Id = device.Id,
            Database = device.Database,
            HubAddress = device.HubAddress,
            HubName = device.HubName,
            Name = device.Name,
            ReadInterval = device.ReadInterval,
            SaveInterval = device.SaveInterval
        };
        if (device.ModbusConfiguration is not null) {
            modbusDevice.ModbusConfig = device.ModbusConfiguration.ToDto();
        }

        if (device.ChannelRegisterMap is not null) {
            modbusDevice.RegisterMapping = device.ChannelRegisterMap.ToDto();
        }

        if (device.NetworkConfiguration is not null) {
            modbusDevice.NetworkConfig = device.NetworkConfiguration.ToDto();
        }

        return modbusDevice;
    }

    public static ModbusConfigDto ToDto(this ModbusConfiguration modbusConfig) {
        return new ModbusConfigDto() {
            Coils = modbusConfig.Coils,
            HoldingRegisters = modbusConfig.HoldingRegisters,
            DiscreteInputs = modbusConfig.DiscreteInputs,
            Id = modbusConfig.Id,
            InputRegisters = modbusConfig.InputRegisters,
            SlaveAddress = modbusConfig.SlaveAddress
        };
    }

    public static NetworkConfigDto ToDto(this NetworkConfiguration netConfig) {
        return new NetworkConfigDto() {
            Id = netConfig.Id,
            IpAddress = netConfig.IpAddress,
            Mac = netConfig.Mac,
            Dns = netConfig.Dns,
            Gateway = netConfig.Gateway,
            Port = netConfig.Port
        };
    }
    public static ChannelMappingConfigDto ToDto(this ModbusChannelRegisterMap registerMap) {
        return new ChannelMappingConfigDto() {
            AlertRegisterType = registerMap.AlertRegisterType,
            AlertStart = registerMap.AlertStart,
            AlertStop = registerMap.AlertStop,
            AnalogRegisterType = registerMap.AnalogRegisterType,
            AnalogStart = registerMap.AnalogStart,
            AnalogStop = registerMap.AnalogStop,
            DiscreteRegisterType = registerMap.DiscreteRegisterType,
            DiscreteStart = registerMap.DiscreteStart,
            DiscreteStop = registerMap.DiscreteStop,
            VirtualRegisterType = registerMap.VirtualRegisterType,
            VirtualStart = registerMap.VirtualStart,
            VirtualStop = registerMap.VirtualStop,
            Id = registerMap.Id
        };
    }

    public static ModbusDevice ToEntity(this ModbusDeviceDto device) {
        var modbusDevice = new ModbusDevice {
            Id = device.Id,
            Database = device.Database,
            HubAddress = device.HubAddress,
            HubName = device.HubName,
            Name = device.Name,
            ReadInterval = device.ReadInterval,
            SaveInterval = device.SaveInterval
        };
        if (device.ModbusConfig is not null) {
            modbusDevice.ModbusConfiguration = device.ModbusConfig.ToEntity();
        }

        if (device.RegisterMapping is not null) {
            modbusDevice.ChannelRegisterMap = device.RegisterMapping.ToEntity();
        }

        if (device.NetworkConfig is not null) {
            modbusDevice.NetworkConfiguration = device.NetworkConfig.ToEntity();
        }
        return modbusDevice;
    }
    
    public static ModbusConfiguration ToEntity(this ModbusConfigDto modbusConfig) {
        return new ModbusConfiguration() {
            Coils = modbusConfig.Coils,
            HoldingRegisters = modbusConfig.HoldingRegisters,
            DiscreteInputs = modbusConfig.DiscreteInputs,
            Id = modbusConfig.Id,
            InputRegisters = modbusConfig.InputRegisters,
            SlaveAddress = modbusConfig.SlaveAddress
        };
    }
    
    public static NetworkConfiguration ToEntity(this NetworkConfigDto netConfig) {
        return new NetworkConfiguration() {
            IpAddress = netConfig.IpAddress,
            Mac = netConfig.Mac,
            Dns = netConfig.Dns,
            Gateway = netConfig.Gateway,
            Port=netConfig.Port,
            Id = netConfig.Id
        };
    }
    
    public static ModbusChannelRegisterMap ToEntity(this ChannelMappingConfigDto registerMap) {
        return new ModbusChannelRegisterMap() {
            AlertRegisterType = registerMap.AlertRegisterType,
            AlertStart = registerMap.AlertStart,
            AlertStop = registerMap.AlertStop,
            AnalogRegisterType = registerMap.AnalogRegisterType,
            AnalogStart = registerMap.AnalogStart,
            AnalogStop = registerMap.AnalogStop,
            DiscreteRegisterType = registerMap.DiscreteRegisterType,
            DiscreteStart = registerMap.DiscreteStart,
            DiscreteStop = registerMap.DiscreteStop,
            VirtualRegisterType = registerMap.VirtualRegisterType,
            VirtualStart = registerMap.VirtualStart,
            VirtualStop = registerMap.VirtualStop,
            Id = registerMap.Id
        };
    }
}

