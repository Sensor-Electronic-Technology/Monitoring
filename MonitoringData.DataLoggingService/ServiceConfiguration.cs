using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.DataLoggingService {
    public class ServiceConfiguration {
        public const string Section = "ServiceConfig";
        public string Database { get; set; } = string.Empty;
        public string DatabaseIP { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
        public string DeviceId { get; set; } = string.Empty;
    }
}
