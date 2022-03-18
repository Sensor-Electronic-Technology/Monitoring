using MonitoringSystem.Shared.Data;

namespace MonitoringConfig.Infrastructure.Data.Model {
    public class Alert {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public bool Bypass { get; set; }
        public int  BypassResetTime { get; set; }
        public bool Enabled { get; set; }
        public AlertItemType AlertItemType { get; set; }
        public ModbusAddress ModbusAddress { get; set; }
        public int? InputChannelId { get; set; }
        public InputChannel InputChannel { get; set; }
    }

    public class AnalogAlert:Alert {
        public ICollection<AnalogLevel> AlertLevels { get; set; }
        //public int? AnalogInputId { get; set; }
        //public AnalogInput AnalogInput { get; set; }
    }

    public class DiscreteAlert : Alert {
        public DiscreteLevel AlertLevel { get; set; }
        //public int? DiscreteInputId { get; set; }
        //public DiscreteInput DiscreteInput { get; set; }
    }
}
