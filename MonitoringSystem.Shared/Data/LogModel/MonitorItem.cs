using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Shared.Data {
    public class MonitorItem {
        public int _id { get; set; }
        public string identifier { get; set; }
        public bool display { get; set; }
        public ObjectId deviceId { get; set; }
    }

    public class AnalogChannel : MonitorItem {
        public int SensorTypeId { get; set; }
        public float recordThreshold { get; set; }
        public int factor { get; set; }
        public int reg { get; set; }
        public int reglen { get; set; }
        public ActionType l1action { get; set; }
        public float l1setpoint { get; set; }
        public ActionType l2action { get; set; }
        public float l2setpoint { get; set; }
        public ActionType l3action { get; set; }
        public float l3setpoint { get; set; }

    }

    public class DiscreteChannel : MonitorItem {
        public ActionType action { get; set; }
        public DiscreteState TriggerOn { get; set; }
    }

    public class OutputItem : MonitorItem {

    }

    public class VirtualChannel : MonitorItem {

    }

    public class ActionItem : MonitorItem {
        public ActionType actionType { get; set; }
        public bool EmailEnabled { get; set; }
        public int EmailPeriod { get; set; }
    }
}
