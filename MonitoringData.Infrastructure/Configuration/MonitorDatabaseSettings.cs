using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Configuration {
    public class MonitorDatabaseSettings {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
