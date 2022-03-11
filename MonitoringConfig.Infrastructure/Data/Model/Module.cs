using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringConfig.Infrastructure.Data.Model {
    public enum ModuleChannel {
        AnalogInput,
        DiscreteInput,
        DiscreteOutput,
        AnalogOutput
    }

    public class Module {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Slot { get; set; }
        public int ChannelCount { get; set; }
        public ModuleChannel ModuleChannel { get; set; }
        public ICollection<MonitoringBox> MonitoringBoxes { get; set; } = new List<MonitoringBox>();
    }
}
