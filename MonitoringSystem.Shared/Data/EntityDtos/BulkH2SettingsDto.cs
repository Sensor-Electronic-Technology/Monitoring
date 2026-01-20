using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringSystem.Shared.Data.EntityDtos;

public class BulkH2SettingsDto {
    public AnalogItem? AnalogItem { get; set; }

    public BulkH2CalcSettings BulkH2CalcSettings { get; set; } = BulkH2CalcSettings.Init();
}