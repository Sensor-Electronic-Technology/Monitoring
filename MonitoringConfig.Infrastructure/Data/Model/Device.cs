using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringConfig.Infrastructure.Data.Model {
    public abstract class Device {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string DisplayName { get; set; }
        public string Status { get; set; }
        public bool BypassAlarms { get; set; }
        public double ReadInterval { get; set; }
        public double SaveInterval { get; set; }
        public string DatabaseName { get; set; }
        public ICollection<FacilityZone> Zones { get; set; } = new List<FacilityZone>();
    }

    public class ApiDevice : Device {
        public string ApiToken { get; set; }
    }

    public class BnetDevice : Device {

    }
}
