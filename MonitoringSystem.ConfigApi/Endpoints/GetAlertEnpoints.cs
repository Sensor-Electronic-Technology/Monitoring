using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Contracts.Requests.Get;
using MonitoringSystem.ConfigApi.Contracts.Responses.Get;
using MonitoringSystem.ConfigApi.Mapping;

namespace MonitoringSystem.ConfigApi.Endpoints;

[HttpGet("alerts/{InputChannelId:guid}"),AllowAnonymous]
public class GetAlertEndpoint:Endpoint<GetAlertRequest,GetAlertResponse> {
    private readonly MonitorContext _context;

    public GetAlertEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(GetAlertRequest req, CancellationToken ct) {
        var alert = await this._context.Alerts
            .FirstOrDefaultAsync(e => e.InputChannelId == req.InputChannelId,ct);
        if (alert is not null) {
            var response = new GetAlertResponse() { Alert = alert.ToDto() };
            await SendOkAsync(response, ct);
        } else {
            await SendNotFoundAsync(ct);
        }
    }
}

/*[HttpGet("alerts/analog/{AnalogChannelId:guid}"),AllowAnonymous]
public class GetAnalogAlertEndpoint:Endpoint<GetAnalogAlertRequest,GetAnalogAlertResponse> {
    private readonly MonitorContext _context;

    public GetAnalogAlertEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(GetAnalogAlertRequest req, CancellationToken ct) {
        var alert = await this._context.Alerts
            .FirstOrDefaultAsync(e => e.InputChannelId == req.AnalogChannelId,ct);
        if (alert is not null) {
            var response = new GetAnalogAlertResponse() { AnalogAlert = alert.ToDto() };
            await SendOkAsync(response, ct);
        } else {
            await SendNotFoundAsync(ct);
        }
    }
}

[HttpGet("alerts/discrete/{DiscreteChannelId:guid}"),AllowAnonymous]
public class GetDiscreteAlertEndpoint:Endpoint<GetDiscreteAlertRequest,GetDiscreteAlertResponse> {
    private readonly MonitorContext _context;

    public GetDiscreteAlertEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(GetDiscreteAlertRequest req, CancellationToken ct) {
        var alert = await this._context.Alerts.OfType<DiscreteAlert>()
            .FirstOrDefaultAsync(e => e.InputChannelId == req.DiscreteChannelId,ct);
        if (alert is not null) {
            var response = new GetDiscreteAlertResponse() { DiscreteAlert = alert.ToDto() };
            await SendOkAsync(response, ct);
        } else {
            await SendNotFoundAsync(ct);
        }
    }
}*/

[HttpGet("alerts/levels/analog/{AnalogAlertId:guid}"),AllowAnonymous]
public class GetAnalogLevelsEndpoint:Endpoint<GetAnalogLevelsRequest,GetAnalogLevelsResponse> {
    private readonly MonitorContext _context;

    public GetAnalogLevelsEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(GetAnalogLevelsRequest req, CancellationToken ct) {
        var levels = await this._context.AlertLevels.OfType<AnalogLevel>()
            .Include(e=>e.DeviceAction)
                .ThenInclude(e=>e.FacilityAction)
            .Where(e => e.AnalogAlertId == req.AnalogAlertId)
            .Select(e=>e.ToDto())
            .ToListAsync(ct);
        if (levels.Any()) {
            var response = new GetAnalogLevelsResponse() { AnalogLevels = levels };
            await SendOkAsync(response, ct);
        } else {
            await SendNotFoundAsync(ct);
        }
    }
}

[HttpGet("alerts/levels/discrete/{DiscreteAlertId:guid}"),AllowAnonymous]
public class GetDiscreteLevelEndpoint:Endpoint<GetDiscreteLevelRequest,GetDiscreteLevelResponse> {
    private readonly MonitorContext _context;

    public GetDiscreteLevelEndpoint(MonitorContext context) {
        this._context = context;
    }
    
    public override async Task HandleAsync(GetDiscreteLevelRequest req, CancellationToken ct) {
        var level = await this._context.AlertLevels.OfType<DiscreteLevel>()
            .Include(e=>e.DeviceAction)
                .ThenInclude(e=>e.FacilityAction)
            .FirstOrDefaultAsync(e => e.DiscreteAlertId == req.DiscreteAlertId,ct);
        if (level is not null) {
            var response = new GetDiscreteLevelResponse() { DiscreteLevel = level.ToDto() };
            await SendOkAsync(response, ct);
        } else {
            await SendNotFoundAsync(ct);
        }
    }
}