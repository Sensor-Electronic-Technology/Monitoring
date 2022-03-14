using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Shared.Data {
    public class MonitorAlertDTO {
        public int channelId { get; set; }
        public DateTime lastAlarm { get; set; }
        public int resetTime { get; set; }
        public bool enabled { get; set; }
        public bool isAnalog { get; set; }
    }
}
