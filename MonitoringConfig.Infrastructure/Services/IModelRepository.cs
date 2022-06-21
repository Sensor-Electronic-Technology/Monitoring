using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringConfig.Infrastructure.Services {
    public interface IModelRepository {
        Task<T> GetDevice<T>(string identifier) where T:Device;
        Task<IList<Channel>> Channels(string deviceId);
        Task<DeviceDto> GetNetworkConfiguration(string deviceId);
    }
}
