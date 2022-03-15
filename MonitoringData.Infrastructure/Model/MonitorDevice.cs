using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Model {
    public class MonitorDevice {
        public string identifier { get; set; }
        public string databaseName { get; set; }
        public NetworkConfiguration NetworkConfiguration { get; set; }
    }
}
