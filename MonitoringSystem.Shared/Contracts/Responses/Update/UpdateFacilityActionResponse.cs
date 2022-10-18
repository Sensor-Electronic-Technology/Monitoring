using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Responses.Update;

public class UpdateFacilityActionResponse {
    public FacilityActionDto FacilityAction { get; set; } = default!;
}

public class UpdateDeviceActionResponse {
    public DeviceActionDto DeviceAction { get; set; } = default!;
}