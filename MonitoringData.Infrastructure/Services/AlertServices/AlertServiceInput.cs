using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringData.Infrastructure.Model;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Services.AlertServices {
    public class ItemAlert  {
        public MonitorAlert Alert { get; set; }
        public float Reading { get; set; }
        public AlertAction AlertAction { get; set; }
        public ItemAlert ActiveAlert { get; set; }

        public ItemAlert(MonitorAlert alert,ActionType state) {
            this.Alert = alert;
            this.Alert.CurrentState = state;
            this.Reading = 0.00f;
        }
    }
}
