namespace MonitoringSystem.Shared.Data.EntityDtos; 

public class AlertDto {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool Bypass { get; set; }
    public int BypassResetTime { get; set; }
    public bool Enabled { get; set; }
    public ModbusAddress? ModbusAddress { get; set; }
}

public class AnalogAlertDto:AlertDto {
    public IEnumerable<AnalogLevelDto> AnalogLevels { get; set; } = Enumerable.Empty<AnalogLevelDto>();
}

public class DiscreteAlertDto : AlertDto {
    public DiscreteLevelDto? DiscreteLevelDto { get; set; }
}

public abstract class AlertLevelDto {
    public Guid Id { get; set; }
    public bool Bypass { get; set; }
    public int BypassResetTime { get; set; }
    public bool Enabled { get; set; }
    public DeviceActionDto? DeviceAction { get; set; }
}

public class AnalogLevelDto : AlertLevelDto {
    public double SetPoint { get; set; }
}

public class DiscreteLevelDto:AlertLevelDto {
    public DiscreteState TriggerOn { get; set; }
}