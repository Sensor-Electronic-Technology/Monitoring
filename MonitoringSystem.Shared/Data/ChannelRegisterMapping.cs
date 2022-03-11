using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Shared.Data {
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
