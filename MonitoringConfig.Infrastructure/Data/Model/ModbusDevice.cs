using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using MonitoringSystem.Shared.Data;

namespace MonitoringConfig.Infrastructure.Data.Model {
   
    public class ModbusDevice:Device {
        public NetworkConfiguration NetworkConfiguration { get; set; }
        public ICollection<Channel> Channels { get; set; } = new List<Channel>();
    }

    public class MonitoringBox:ModbusDevice {
        public ICollection<ModbusActionMap> ModbusActionMapping { get; set; }
        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}
