using System.Collections.ObjectModel;

namespace MonitoringConfig.Infrastructure.Data.Model {
    public class Sensor {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public double RecordThreshold { get; set; }
        public double Slope { get; set; }
        public double Offset { get; set; }
        public double Factor { get; set; }
        public string Units { get; set; }
        public int YAxisMin { get; set; }
        public int YAxitMax { get; set; }
        public ICollection<AnalogInput> AnalogInputs { get; set; } = new List<AnalogInput>();
    }
}
