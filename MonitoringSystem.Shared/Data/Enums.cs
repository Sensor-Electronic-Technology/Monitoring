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
}
