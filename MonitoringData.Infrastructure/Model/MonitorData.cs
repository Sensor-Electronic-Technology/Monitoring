using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Model {
    public class ChannelReading {
        public int channelid { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class AnalogReading : ChannelReading {
        public double value { get; set; }
    }

    public class DiscreteReading : ChannelReading {
        public bool value { get; set; }
    }

    public class OutputReading : ChannelReading {
        public bool value { get; set; }
    }

    public class VirtualReading : ChannelReading {
        public bool value { get; set; }
    }

    public class ActionReading {
        public int actionid { get; set; }
        public DateTime timestamp { get; set; }
        public bool value { get; set; }
    }

    public class AlertReading {
        public int alertid { get; set; }
        public DateTime timestamp { get; set; }
        public int value { get; set; }
    }
}
