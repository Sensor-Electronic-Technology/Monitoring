using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Model {
    public class MonitorAction {
        public int _id { get; set; }
        public ActionType ActionType { get; set; }
        public bool EmailEnabled { get; set; }
        public bool EmailPeriod { get; set; }
    }
}
