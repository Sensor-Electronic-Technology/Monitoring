using MonitoringSystem.Shared.Data;

namespace MonitoringWeb.ControlService.Hubs; 

public interface IModbusControlHub {
    Task Toggle(string device);
    Task InitializeActions(IEnumerable<RemoteAction> remoteActions);
}