using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Contracts.Responses.Get;

public class GetFacilityActionsResponse {
    public IEnumerable<FacilityActionDto> FacilityActions { get; set; } = default!;
}

public class GetDeviceActionsResponse {
    public IEnumerable<DeviceActionDto>? DeviceActions { get; set; } = Enumerable.Empty<DeviceActionDto>();
}

public class GetDeviceActionResponse {
    public DeviceActionDto DeviceAction { get; set; } = default!;
}