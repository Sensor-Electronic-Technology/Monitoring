namespace MonitoringSystem.Shared.Data.SettingsModel;
public abstract class MonitorSettings {
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}