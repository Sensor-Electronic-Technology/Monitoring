using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Shared.SignalR {

    public class MonitorData {
        public DateTime TimeStamp { get; set; }
        public List<ItemStatus> data { get; set; }
        public List<ItemStatus> analogData { get; set; }
        public List<ItemStatus> discreteData { get; set; }
        public List<ItemStatus> virtualData { get; set; }

    }

    public class ItemStatus {
        public string Item { get; set; }
        public string State { get; set; }
        public string Value { get; set; }
    }

    //public interface IMonitorHub {
    //    Task ShowCurrent(IList<ItemStatus> items);
    //}

    //public class MonitorHub : Hub<IMonitorHub> {
    //    public async Task SendDataToClients(IList<ItemStatus> items) {
    //        await Clients.All.ShowCurrent(items);
    //    }
    //}

    public interface IMonitorHub {
        Task ShowCurrent(MonitorData data);
    }

    public class MonitorHub : Hub<IMonitorHub> {
        public async Task SendDataToClients(MonitorData data) {
            await Clients.All.ShowCurrent(data);
        }
    }
}
