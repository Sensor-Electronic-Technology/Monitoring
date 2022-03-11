using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Shared.Data {
    public class NetworkConfiguration {
        public string IPAddress { get; set; }
        public string DNS { get; set; }
        public string MAC { get; set; }
        public string Gateway { get; set; }
        public int Port { get; set; }
        public ModbusConfig ModbusConfig { get; set; }
    }
}
