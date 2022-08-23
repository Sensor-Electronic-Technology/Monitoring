using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Contracts.Requests.Get;
using MonitoringSystem.ConfigApi.Contracts.Responses.Get;
using MonitoringSystem.ConfigApi.Mapping;

namespace MonitoringSystem.ConfigApi.Endpoints;

[HttpGet("channels/DeviceId"),AllowAnonymous]
public class GetDeviceChannelsEndpoint : Endpoint<GetDeviceChannelsRequest,GetDeviceChannelsResponse> {
    private readonly MonitorContext _context;

    public GetDeviceChannelsEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(GetDeviceChannelsRequest req, CancellationToken ct) {
        var analogChannels = await this._context.Channels.OfType<AnalogInput>()
            .Include(e => e.Sensor)
            .Include(e => e.Alert)
                .ThenInclude(e => ((AnalogAlert)e).AlertLevels)
                    .ThenInclude(e => e.DeviceAction)
            .Where(e => e.ModbusDeviceId == req.DeviceId)
            .Select(e=>e.ToDto())
            .ToListAsync(ct);

        var response = new GetDeviceChannelsResponse() { AnalogInputs = analogChannels.AsEnumerable() };
        await SendOkAsync(response, ct);
    }
}