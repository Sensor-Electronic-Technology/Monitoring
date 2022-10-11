namespace MonitoringConfig.Data.Model;

public class DeviceAction {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int FirmwareId { get; set; }
    public Guid ModbusDeviceId { get; set; }
    public ModbusDevice? ModbusDevice { get; set; }
    public Guid FacilityActionId { get; set; }
    public FacilityAction? FacilityAction { get; set; }
    public ICollection<ActionOutput> ActionOutputs { get; set; } = new List<ActionOutput>();
    public ICollection<AlertLevel> AlertLevels { get; set; } = new List<AlertLevel>();
}