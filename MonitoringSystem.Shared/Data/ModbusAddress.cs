using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Shared.Data {
    public class ModbusAddress {
        public int Address { get; set; }
        public int RegisterLength { get; set; }
        public ModbusRegister RegisterType { get; set; }
    }
}
