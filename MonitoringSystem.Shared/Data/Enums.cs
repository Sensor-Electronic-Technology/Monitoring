using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Shared.Data {
    public enum DiscreteState {
        High = 1, Low = 0
    }

    public enum ModbusRegister {
        Holding = 1,
        Coil = 2,
        DiscreteInput = 3,
        Input = 4
    }

    public enum ActionType {
        Okay = 6,
        Alarm = 5,
        Warning = 4,
        SoftWarn = 3,
        Maintenance = 2,
        Custom = 1
    }

    public enum DeviceState {
        ALARM=0,
        WARNING=1,
        MAINTENANCE=2,
        OKAY=3
    }

    public enum AlertItemType {
        Analog,
        Discrete,
        Virtual
    }

    public enum AlertTriggerType {
        Firmware,
        Software
    }

}
