using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Contracts.Requests.Update;
using MonitoringSystem.ConfigApi.Contracts.Responses.Update;
using MonitoringSystem.ConfigApi.Mapping;

namespace MonitoringSystem.ConfigApi.Endpoints;

[HttpPut("actions/facility/FacilityAction"),AllowAnonymous]
public class UpdateFacilityActionEndpoint : Endpoint<UpdateFacilityActionRequest, UpdateFacilityActionResponse> {
    private readonly MonitorContext _context;

    public UpdateFacilityActionEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateFacilityActionRequest req, CancellationToken ct) {
        var facilityAction = req.FacilityAction.ToEntity();
        this._context.Update(facilityAction);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateFacilityActionResponse(){FacilityAction = facilityAction.ToDto()},ct);
        } else {
            await SendErrorsAsync(400, ct);
        }
    }
}

[HttpPut("actions/device/DeviceAction"),AllowAnonymous]
public class UpdateDeviceActionEndpoint : Endpoint<UpdateDeviceActionRequest, UpdateDeviceActionResponse> {
    private readonly MonitorContext _context;

    public UpdateDeviceActionEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateDeviceActionRequest req, CancellationToken ct) {
        var deviceAction = req.DeviceAction.ToEntity();
        this._context.Update(deviceAction);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateDeviceActionResponse(){DeviceAction = deviceAction.ToDto()},ct);
        } else {
            await SendErrorsAsync(400, ct);
        }
    }
}