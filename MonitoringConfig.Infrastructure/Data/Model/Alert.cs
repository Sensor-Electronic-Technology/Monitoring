using MonitoringSystem.Shared.Data;

namespace MonitoringConfig.Infrastructure.Data.Model {
    public class Alert {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public bool Bypass { get; set; } //remove
        public int  BypassResetTime { get; set; } //remove
        public bool Enabled { get; set; }
        public AlertItemType AlertItemType { get; set; }
        public ModbusAddress ModbusAddress { get; set; }
        public int? InputChannelId { get; set; }
        public InputChannel InputChannel { get; set; }
    }

    public class AnalogAlert:Alert {
        public ICollection<AnalogLevel> AlertLevels { get; set; }

    }

    public class DiscreteAlert : Alert {
        public DiscreteLevel AlertLevel { get; set; }
    }
}
