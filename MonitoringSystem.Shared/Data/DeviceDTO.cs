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
}
