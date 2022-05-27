using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MonitoringConfig.Infrastructure.Services {
    public interface IMonitorDataUpdater {
        Task UpdateDevice(Device device);
        Task UpdateChannel(AnalogInput input);
        Task UpdateChannel(DiscreteInput input);
        Task UpdateChannel(VirtualInput input);
        Task UpdateChannel(OutputChannel output);    
    }

    public class MonitorDataUpdater : IMonitorDataUpdater {
        public Task UpdateChannel(AnalogInput input) {
            throw new NotImplementedException();
        }

        public Task UpdateChannel(DiscreteInput input) {
            throw new NotImplementedException();
        }

        public Task UpdateChannel(VirtualInput input) {
            throw new NotImplementedException();
        }

        public Task UpdateChannel(OutputChannel output) {
            throw new NotImplementedException();
        }

        public Task UpdateDevice(Device device) {
            throw new NotImplementedException();
        }
    }
}
