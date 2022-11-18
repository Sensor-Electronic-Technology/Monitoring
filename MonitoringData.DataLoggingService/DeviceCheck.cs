using System.Net.NetworkInformation;
using System.Text;
using MassTransit.Mediator;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Data;
using MonitoringData.Infrastructure.Events;
using MonitoringData.Infrastructure.Services;
using MonitoringSystem.Shared.Services;
namespace MonitoringData.DataLoggingService; 

public class DeviceCheck:IHostedService, IDisposable {
    private ILogger<DeviceCheck> _logger;
    private System.Timers.Timer _timer;
    private readonly IMonitorConfigurationProvider _configProvider;
    private readonly MonitorDataLogSettings _settings;
    private readonly IMediator _mediator;
    
    public DeviceCheck(DataLogConfigProvider configProvider, 
        IMediator mediator, 
        ILogger<DeviceCheck> logger) {
        this._configProvider = configProvider;
        this._mediator = mediator;
        this._logger = logger;
        this._settings = configProvider.MonitorDataLogSettings;
        this._timer = new(interval: 30000);
        this._timer.Elapsed += async (sender, e) => {
            await this.Handler();
        };
    }
    
    public Task StartAsync(CancellationToken cancellationToken) {
        this._timer.Start();
        return Task.CompletedTask;
    }

    private async Task Handler() {
        
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        this._timer.Stop();
        return Task.CompletedTask;
    }

    public void Dispose() {
        if (this._timer != null) {
            this._timer.Dispose();
        }
    }
}