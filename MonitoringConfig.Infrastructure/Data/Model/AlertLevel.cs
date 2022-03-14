using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringSystem.Shared.Data;

namespace MonitoringConfig.Infrastructure.Data.Model {

    public abstract class AlertLevel {
        public int Id { get; set; }
        public bool Bypass { get; set; }
        public int BypassResetTime { get; set; }
        public bool Enabled { get; set; }
        public int? FacilityActionId { get; set; }
        public FacilityAction FacilityAction { get; set; }
        
    }

    public class AnalogLevel : AlertLevel {
        public double SetPoint { get; set; }
        public int? AnalogAlertId { get; set; }
        public AnalogAlert AnalogAlert { get; set; }
    }

    public class DiscreteLevel : AlertLevel {
        public DiscreteState TriggerOn { get; set; }
        public int? DiscrteAlertId { get; set; }
        public DiscreteAlert DiscreteAlert { get; set; }
    }
}
