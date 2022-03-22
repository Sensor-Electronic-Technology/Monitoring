using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringData.Infrastructure.Model;

namespace MonitoringData.Infrastructure.Services.AlertServices {
    public class ItemAlert  {
        public MonitorAlert Alert { get; set; }
        public AlertReading AlertReading { get; set; }
        public float Reading { get; set; }
        public AlertAction AlertAction { get; set; }
        public ItemAlert ActiveAlert { get; set; }
    }
}
