using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Contracts.Requests.Update;
using MonitoringSystem.ConfigApi.Contracts.Responses.Update;
using MonitoringSystem.ConfigApi.Mapping;

namespace MonitoringSystem.ConfigApi.Endpoints;

[HttpPut("alerts/Alert"),AllowAnonymous]
public class UpdateAlertEndpoint : Endpoint<UpdateAlertRequest, UpdateAlertResponse> {
    private readonly MonitorContext _context;

    public UpdateAlertEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateAlertRequest req, CancellationToken ct) {
        var alert = req.Alert.ToEntity();
        this._context.Update(alert);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateAlertResponse() { Alert = alert.ToDto() },ct);
        } else {
            await SendErrorsAsync(400,ct);
        }
    }
}

/*[HttpPut("alerts/analog/AnalogAlert"),AllowAnonymous]
public class UpdateAnalogAlertEndpoint : Endpoint<UpdateAnalogAlertRequest, UpdateAnalogAlertResponse> {
    private readonly MonitorContext _context;

    public UpdateAnalogAlertEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateAnalogAlertRequest req, CancellationToken ct) {
        var analogInput = req.AnalogAlert.ToEntity();
        this._context.Update(analogInput);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateAnalogAlertResponse() { AnalogAlert = analogInput.ToDto() },ct);
        } else {
            await SendErrorsAsync(400,ct);
        }
    }
}

[HttpPut("alerts/discrete/DiscreteAlert"),AllowAnonymous]
public class UpdateDiscreteAlertEndpoint : Endpoint<UpdateDiscreteAlertRequest, UpdateDiscreteAlertResponse> {
    private readonly MonitorContext _context;

    public UpdateDiscreteAlertEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateDiscreteAlertRequest req, CancellationToken ct) {
        var analogInput = req.DiscreteAlert.ToEntity();
        this._context.Update(analogInput);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateDiscreteAlertResponse() { DiscreteAlert = analogInput.ToDto() },ct);
        } else {
            await SendErrorsAsync(400,ct);
        }
    }
}*/

[HttpPut("alerts/levels/analog/AnalogLevel"),AllowAnonymous]
public class UpdateAnalogLevelEndpoint : Endpoint<UpdateAnalogLevelRequest, UpdateAnalogLevelResponse> {
    private readonly MonitorContext _context;

    public UpdateAnalogLevelEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateAnalogLevelRequest req, CancellationToken ct) {
        var analogInput = req.AnalogLevel.ToEntity();
        this._context.Update(analogInput);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateAnalogLevelResponse() { AnalogLevel = analogInput.ToDto() },ct);
        } else {
            await SendErrorsAsync(400,ct);
        }
    }
}

[HttpPut("alerts/levels/discrete/DiscreteLevel"),AllowAnonymous]
public class UpdateDiscreteLevelEndpoint : Endpoint<UpdateDiscreteLevelRequest, UpdateDiscreteLevelResponse> {
    private readonly MonitorContext _context;

    public UpdateDiscreteLevelEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateDiscreteLevelRequest req, CancellationToken ct) {
        var analogInput = req.DiscreteLevel.ToEntity();
        this._context.Update(analogInput);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateDiscreteLevelResponse() { DiscreteLevel = analogInput.ToDto() },ct);
        } else {
            await SendErrorsAsync(400,ct);
        }
    }
}


