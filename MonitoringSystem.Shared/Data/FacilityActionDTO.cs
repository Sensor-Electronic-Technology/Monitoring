using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Shared.Data {
    public class FacilityActionDTO {
        public int Id { get; set;}
        public ActionType ActionType { get; set; }
        public bool EmailEnabled { get; set; }
        public int EmailPeriod { get; set; }
    }
}
