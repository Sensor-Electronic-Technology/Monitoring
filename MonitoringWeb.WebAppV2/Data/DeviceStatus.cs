﻿using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.SignalR;

namespace MonitoringWeb.WebAppV2.Data; 

public class DeviceStatus {
    public string DeviceName { get; set; }
    public ActionType Status { get; set; }
    public IEnumerable<ItemStatus> ActiveAlerts { get; set; }
}