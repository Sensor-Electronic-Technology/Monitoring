using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Contracts.Requests.Get;
using MonitoringSystem.ConfigApi.Contracts.Responses.Get;
using MonitoringSystem.ConfigApi.Mapping;

namespace MonitoringSystem.ConfigApi.Endpoints;

[HttpGet("actions/facility"),AllowAnonymous]
public class GetFacilityActionsEndpoint : EndpointWithoutRequest<GetFacilityActionsResponse> {
    private readonly MonitorContext _context;

    public GetFacilityActionsEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(CancellationToken ct) {
        var facilityActions = await this._context.FacilityActions.Select(e => e.ToDto()).ToListAsync(ct);
        if (facilityActions.Any()) {
            await SendOkAsync(new GetFacilityActionsResponse() { FacilityActions = facilityActions }, ct);
        } else {
            await SendNotFoundAsync(ct);
        }
    }
}

[HttpGet("actions/deviceactions/{DeviceId:guid}"),AllowAnonymous]
public class GetAllDeviceActionsEndpoint : Endpoint<GetDeviceActionsRequest,GetDeviceActionsResponse> {
    private readonly MonitorContext _context;

    public GetAllDeviceActionsEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(GetDeviceActionsRequest req,CancellationToken ct) {
        var deviceActions = await this._context.DeviceActions
            .Include(e=>e.FacilityAction)
            .Where(e=>e.ModbusDeviceId==req.DeviceId)
            .Select(e => e.ToDto())
            .ToListAsync(ct);
        
        if (deviceActions.Any()) {
            await SendOkAsync(new GetDeviceActionsResponse() { DeviceActions = deviceActions }, ct);
        } else {
            await SendNotFoundAsync(ct);
        }
    }
}

[HttpGet("actions/deviceaction/{DeviceActionId:guid}"),AllowAnonymous]
public class GetDeviceActionEndpoint : Endpoint<GetDeviceActionRequest,GetDeviceActionResponse> {
    private readonly MonitorContext _context;

    public GetDeviceActionEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(GetDeviceActionRequest req,CancellationToken ct) {
        var deviceAction = await this._context.DeviceActions
            .Include(e=>e.FacilityAction)
            .FirstOrDefaultAsync(e => e.Id == req.DeviceActionId, ct);
        if (deviceAction is not null) {
            await SendOkAsync(new GetDeviceActionResponse() { DeviceAction = deviceAction.ToDto() }, ct);
        } else {
            await SendNotFoundAsync(ct);
        }
    }
}