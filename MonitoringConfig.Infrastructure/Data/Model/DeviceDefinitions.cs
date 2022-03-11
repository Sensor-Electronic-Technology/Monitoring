using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringConfig.Infrastructure.Data.Model {
    public class ApiDevice:Device {
        public string ApiToken { get; set; }
    }

    public class BnetDevice : Device {

    }
}
