using MonitoringWeb.ControlService.Hubs;

namespace MonitoringWeb.ControlService.Services;

public class HubService:IHostedService,IDisposable {
    private readonly MonitorControlHub _controlHub;

    public HubService(MonitorControlHub controlHub) {
        this._controlHub = controlHub;
    }
    public async Task StartAsync(CancellationToken cancellationToken) {
        await this._controlHub.StartAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public void Dispose() {
        this._controlHub.Dispose();
    }
}