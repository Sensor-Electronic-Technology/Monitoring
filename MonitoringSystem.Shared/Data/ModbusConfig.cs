using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Shared.Data {
    public class ModbusConfig {
        public int SlaveAddress { get; set; }
        public int DiscreteInputs { get; set; }
        public int InputRegisters { get; set; }
        public int HoldingRegisters { get; set; }
        public int Coils { get; set; }
        public ModbusAddress ModbusAddress { get; set; }
        public ChannelRegisterMapping ChannelMapping { get; set; }
    }
}
