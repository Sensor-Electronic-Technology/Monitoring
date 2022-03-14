using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Model {
    public class MonitorAlert {
        public int _id { get; set; }
        public int channelId { get; set; }
        public bool enabled { get; set; }
        public bool bypassed { get; set; }
        public int bypassResetTime { get; set; }
        public DateTime lastAlarm { get; set; }
        public bool latched { get; set; }
        public ActionType CurrentState { get; set; }
    }

    //public class MonitorAnalogAlert : MonitorAlert {
    //    public ActionType level1Type { get; set; }
    //    public bool level1Latched { get; set; }
    //    public bool level1bypass { get; set; }
    //    public int level1EmailPeriod { get; set; }
    //    public bool level1EmailEnabled { get; set; }

    //    public ActionType level2Type { get; set; }
    //    public bool level2Latched { get; set; }
    //    public bool level2bypass { get; set; }
    //    public int level2EmailPeriod { get; set; }
    //    public bool level2EmailEnabled { get; set; }

    //    public ActionType level3Type { get; set; }
    //    public bool level3Latched { get; set; }
    //    public bool level3bypass { get; set; }
    //    public int level3EmailPeriod { get; set; }
    //    public bool level3EmailEnabled { get; set; }
    //}

    //public class MonitorDiscreteAlert : MonitorAlert {
    //    public ActionType type { get; set; }
    //    public bool latched { get; set; }
    //    public bool bypass { get; set; }
    //    public int emailPeriod { get; set; }
    //    public bool emailEnabled { get; set; }
    //}
}
