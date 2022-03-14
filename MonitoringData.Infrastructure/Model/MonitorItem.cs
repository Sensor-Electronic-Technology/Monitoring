using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Model {
    public class MonitorItem {
        public int _id { get; set; }
        public string identifier { get; set; }
    }

    public class AnalogChannel : MonitorItem {

    }

    public class DiscreteChannel : MonitorItem {

    }

    public class OutputChannel : MonitorItem {

    }

    public class VirtualChannel : MonitorItem {

    }

    public class ActionItem : MonitorItem {

    }

    public class MonitorDevice : MonitorItem {

    }
}
