using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Services {
    public interface IMonitorDataRepo {
        Task CreateDatabase(string deviceId);
        Task AddAnalogItems(IList<MonitorItemDTO> items);
        Task AddDiscreteItems(IList<MonitorItemDTO> items);
        Task AddOutputItems(IList<MonitorItemDTO> items);
        Task AddVirtualItems(IList<MonitorItemDTO> items);
        Task AddActionItems(IList<MonitorItemDTO> items);
        Task AddDeviceItem(DeviceDTO device);
        Task AddMonitorAlerts(IList<MonitorAlertDTO> monitorAlerts);


    }
}
