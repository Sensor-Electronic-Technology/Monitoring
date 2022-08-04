using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;

namespace MonitoringSystem.Shared.Services;

public interface IMonitorConfigurationProvider {
    public Task Load();
    public Task Reload();
}
