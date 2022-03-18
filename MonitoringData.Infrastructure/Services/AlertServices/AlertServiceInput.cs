using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringData.Infrastructure.Model;

namespace MonitoringData.Infrastructure.Services.AlertServices {
    public class AlertServiceInput {
        List<ItemAlert> Alerting { get; set; }
        List<ItemAlert> SystemStatus { get; set; }
    }

    public class ItemAlert {
        public MonitorAlert Alert { get; set; }
        public string ItemName { get; set; }
        public float Reading { get; set; }
    }
}
