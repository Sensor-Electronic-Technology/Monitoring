﻿using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringConfig.Data.Model; 

public class Alert {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool Bypass { get; set; }
    public int BypassResetTime { get; set; }
    public bool Enabled { get; set; }
    public ModbusAddress? ModbusAddress { get; set; }
    public Guid InputChannelId { get; set; }
    public InputChannel? InputChannel { get; set; }
}

public class AnalogAlert : Alert {
    public ICollection<AnalogLevel> AlertLevels { get; set; } = new List<AnalogLevel>();
}

public class DiscreteAlert : Alert {
    public DiscreteLevel? AlertLevel { get; set; }
}