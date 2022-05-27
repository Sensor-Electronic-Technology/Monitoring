using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data {
    public class MonitorDevice {
        public ObjectId _id { get; set; }
        public DateTime Created { get; set; }
        public string identifier { get; set; }
        public int recordInterval { get; set; }
        public NetworkConfiguration NetworkConfiguration { get; set; }
    }
}
