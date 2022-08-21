using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringData.DataApi.Contracts.Responses; 

public class GetAllManagedDevicesResponse {
    public IEnumerable<ManagedDevice> ManagedDevices { get; set; } = Enumerable.Empty<ManagedDevice>();
}