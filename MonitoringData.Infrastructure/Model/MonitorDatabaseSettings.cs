using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Model {
    public class MonitorDatabaseSettings {

        public static string FileName = "dbSettings.json";
        public static string SectionName = "MonitorDatabaseSettings";
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string EmailSubject { get; set; } = null!;
        public string HubName { get; set; } = null!;
        public string AlertItemCollection { get; set; } = null!;
        public string ActionItemCollection { get; set; } = null!;
        public string AnalogItemCollection { get; set; } = null!;
        public string DiscreteItemCollection { get; set; } = null!;
        public string OutputItemCollection { get; set; } = null!;
        public string VirtualItemColleciton { get; set; } = null!;
        public string MonitorDeviceCollection { get; set; } = null!;

        public string AlertReadingCollection { get; set; } = null!;
        public string ActionReadingCollection { get; set; } = null!;
        public string AnalogReadingCollection { get; set; } = null!;
        public string DiscreteReadingCollection { get; set; } = null!;
        public string VirtualReadingCollection { get; set; } = null!;
        public string OutputReadingCollection { get; set; } = null!;

        public string DeviceReadingCollection { get; set; } = null!;
    }
}
