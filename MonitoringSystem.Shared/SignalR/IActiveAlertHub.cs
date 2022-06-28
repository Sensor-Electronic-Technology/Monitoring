using Microsoft.AspNetCore.SignalR;
using MonitoringSystem.Shared.Data;

namespace MonitoringSystem.Shared.SignalR;
public interface IActiveAlertHub {
    Task ShowActiveAlerts(IEnumerable<AlertDto> alerts);
}

public class ActiveAlertHub : Hub<IActiveAlertHub> {
    
    public async Task SendDataToClients(IEnumerable<AlertDto> alerts) {
        await Clients.All.ShowActiveAlerts(alerts);
    }
}