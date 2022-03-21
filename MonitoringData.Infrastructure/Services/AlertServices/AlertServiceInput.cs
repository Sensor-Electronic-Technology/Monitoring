using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringData.Infrastructure.Model;

namespace MonitoringData.Infrastructure.Services.AlertServices {
    public record ItemAlert  {
        public MonitorAlert Alert { get; set; }
        public float Reading { get; set; }
    }


}
