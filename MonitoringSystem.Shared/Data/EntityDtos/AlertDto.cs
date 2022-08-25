namespace MonitoringSystem.Shared.Data.EntityDtos; 

public class AlertDto {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool Bypass { get; set; }
    public int BypassResetTime { get; set; }
    public bool Enabled { get; set; }
    public int Register { get; set; }
    public int RegisterLength { get; set; }
    public ModbusRegister RegisterType { get; set; }
    public Guid InputChannelId { get; set; }
}

public class AnalogAlertDto:AlertDto { }

public class DiscreteAlertDto : AlertDto { }

public abstract class AlertLevelDto {
    public Guid Id { get; set; }
    public bool Bypass { get; set; }
    public int BypassResetTime { get; set; }
    public bool Enabled { get; set; }
    public Guid? DeviceActionId { get; set; }
}

public class AnalogLevelDto : AlertLevelDto {
    public Guid AnalogAlertId { get; set; }
    public double SetPoint { get; set; }
}

public class DiscreteLevelDto:AlertLevelDto {
    public Guid DiscreteAlertId { get; set; }
    public DiscreteState TriggerOn { get; set; }
}