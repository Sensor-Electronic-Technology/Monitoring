using Microsoft.EntityFrameworkCore;
using MonitoringSystem.Shared.Data;
using System.Collections.ObjectModel;

namespace MonitoringConfig.Infrastructure.Data.Model {
    [Owned]
    public class ActionOutput {
        public DiscreteOutput Output { get; set; }
        public DiscreteState OnLevel { get; set; }
        public DiscreteState OffLevel{ get; set; }
    }

    public class ModbusActionMap {
        public int Id { get; set; }
        public int MonitoringBoxId { get; set; }
        public MonitoringBox MonitoringBox { get; set; }
        public int FacilityActionId { get; set; }
        public FacilityAction FacilityAction { get; set; }
        public ModbusAddress ModbusAddress { get; set; }
    }

    public class FacilityAction {
        public int Id { get; set; }
        public string ActionName { get; set; }
        public bool EmailEnabled { get; set; }
        public int EmailPeriod { get; set; }
        public ActionType ActionType { get; set; }
        public ICollection<ModbusActionMap> ModbusActionMapping { get; set; } = new List<ModbusActionMap>();
        public ICollection<ActionOutput> ActionOutputs { get; set; } = new List<ActionOutput>();
        public ICollection<AlertLevel> AlertLevels { get; set; } = new List<AlertLevel>();

    }
}
