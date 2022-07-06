using MonitoringWeb.ControlService.Hubs;

namespace MonitoringWeb.ControlService.Services;

public class HubService:IHostedService {
    private readonly MonitorControlHub _controlHub;

    public HubService(MonitorControlHub controlHub) {
        this._controlHub = controlHub;
    }
    public Task StartAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
}