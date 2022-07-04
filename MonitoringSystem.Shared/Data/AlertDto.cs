﻿namespace MonitoringSystem.Shared.Data; 

public class AlertDto {
    public int alertId { get; set; }
    public int channelId { get; set; }
    public AlertItemType ItemType { get; set; }
    public string Name { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Device { get; set; }
    public string database { get; set; }
    public string State { get; set; }
    public float Value { get; set; }
}