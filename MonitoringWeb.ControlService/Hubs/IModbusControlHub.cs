using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringWeb.ControlService.Hubs; 

public interface IModbusControlHub {
    Task Toggle(string device);
    Task InitializeActions(IEnumerable<RemoteAction> remoteActions);
}