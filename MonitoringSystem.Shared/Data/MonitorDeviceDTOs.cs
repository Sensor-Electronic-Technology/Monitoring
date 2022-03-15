using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Shared.Data {
    public class DeviceDTO {
        public string Identifier { get; set; }
        public string DatabaseName { get; set; }
        public ChannelRegisterMapping Mapping { get; set; }
        public NetworkConfiguration NetworkConfiguration { get; set; }
    }

    public class NetworkConfiguration {
        public string IPAddress { get; set; }
        public string DNS { get; set; }
        public string MAC { get; set; }
        public string Gateway { get; set; }
        public int Port { get; set; }
        public ModbusConfig ModbusConfig { get; set; }
    }

    public class ModbusAddress {
        public int Address { get; set; }
        public int RegisterLength { get; set; }
        public ModbusRegister RegisterType { get; set; }
    }

    public class ModbusConfig {
        public int SlaveAddress { get; set; }
        public int DiscreteInputs { get; set; }
        public int InputRegisters { get; set; }
        public int HoldingRegisters { get; set; }
        public int Coils { get; set; }
        public ModbusAddress ModbusAddress { get; set; }
        public ChannelRegisterMapping ChannelMapping { get; set; }
    }

    public class ChannelRegisterMapping {
        public ModbusRegister AlertRegisterType { get; set; }
        public int AlertStart { get; set; }
        public int AlertStop { get; set; }

        public ModbusRegister AnalogRegisterType { get; set; }
        public int AnalogStart { get; set; }
        public int AnalogStop { get; set; }

        public ModbusRegister DiscreteRegisterType { get; set; }
        public int DiscreteStart { get; set; }
        public int DiscreteStop { get; set; }

        public ModbusRegister VirtualRegisterType { get; set; }
        public int VirtualStart { get; set; }
        public int VirtualStop { get; set; }

        public ModbusRegister DeviceRegisterType { get; set; }
        public int DeviceStart { get; set; }
        public int DeviceStop { get; set; }

        public ModbusRegister OutputRegisterType { get; set; }
        public int OutputStart { get; set; }
        public int OutputStop { get; set; }

        public ModbusRegister ActionRegisterType { get; set; }
        public int ActionStart { get; set; }
        public int ActionStop { get; set; }
    }
}
