using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Mapping;
using MonitoringSystem.Shared.Contracts.Requests.Update;
using MonitoringSystem.Shared.Contracts.Responses.Update;

namespace MonitoringSystem.ConfigApi.Endpoints;

[HttpPut("channels/analog/AnalogChannel"),AllowAnonymous]
public class UpdateAnalogChannelEndpoint : Endpoint<UpdateAnalogChannelRequest, UpdateAnalogChannelResponse> {
    private readonly MonitorContext _context;

    public UpdateAnalogChannelEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateAnalogChannelRequest req, CancellationToken ct) {
        var channelEntity = req.AnalogChannel.ToEntity();
        this._context.Update(channelEntity);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateAnalogChannelResponse() { AnalogChannel = channelEntity.ToDto() },ct);
        } else {
            await SendErrorsAsync(400, ct);
        }
    }
}
[HttpPut("channels/discrete/DiscreteChannel"),AllowAnonymous]
public class UpdateDiscreteChannelEndpoint : Endpoint<UpdateDiscreteChannelRequest, UpdateDiscreteChannelResponse> {
    private readonly MonitorContext _context;

    public UpdateDiscreteChannelEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateDiscreteChannelRequest req, CancellationToken ct) {
        var channelEntity = req.DiscreteChannel.ToEntity();
        this._context.Update(channelEntity);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateDiscreteChannelResponse() { DiscreteChannel = channelEntity.ToDto() },ct);
        } else {
            await SendErrorsAsync(400, ct);
        }
    }
}
[HttpPut("channels/virtual/VirtualChannel"),AllowAnonymous]
public class UpdateVirtualChannelEndpoint : Endpoint<UpdateVirtualChannelRequest, UpdateVirtualChannelResponse> {
    private readonly MonitorContext _context;

    public UpdateVirtualChannelEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateVirtualChannelRequest req, CancellationToken ct) {
        var channelEntity = req.VirtualChannel.ToEntity();
        this._context.Update(channelEntity);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateVirtualChannelResponse() { VirtualChannel = channelEntity.ToDto() },ct);
        } else {
            await SendErrorsAsync(400, ct);
        }
    }
}
[HttpPut("channels/output/OutputChannel"),AllowAnonymous]
public class UpdateOutputChannelEndpoint : Endpoint<UpdateOutputChannelRequest, UpdateOutputChannelResponse> {
    private readonly MonitorContext _context;

    public UpdateOutputChannelEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateOutputChannelRequest req, CancellationToken ct) {
        var channelEntity = req.OutputChannel.ToEntity();
        this._context.Update(channelEntity);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateOutputChannelResponse() { OutputChannel = channelEntity.ToDto() },ct);
        } else {
            await SendErrorsAsync(400, ct);
        }
    }
}