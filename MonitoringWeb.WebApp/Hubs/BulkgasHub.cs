using Microsoft.AspNetCore.SignalR;

namespace MonitoringWeb.WebApp.Hubs; 

public class BulkGasHub:Hub {
    public async Task SendRefreshRequest() {
        await Clients.All.SendAsync("ReceiveRefreshRequest");
    }
}