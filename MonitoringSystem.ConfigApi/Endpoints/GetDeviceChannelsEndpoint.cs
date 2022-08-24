using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Contracts.Requests.Get;
using MonitoringSystem.ConfigApi.Contracts.Responses.Get;
using MonitoringSystem.ConfigApi.Mapping;

namespace MonitoringSystem.ConfigApi.Endpoints;

[HttpGet("channels/{id:guid}"),AllowAnonymous]
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
            .Where(e => e.ModbusDeviceId == req.Id)
            .Select(e=>e.ToDto())
            .ToListAsync(ct);

        var discreteChannels = await this._context.Channels.OfType<DiscreteInput>()
            .Include(e => e.Alert)
            .ThenInclude(e => ((DiscreteAlert)e).AlertLevel)
            .ThenInclude(e => e.DeviceAction)
            .Where(e => e.ModbusDeviceId == req.Id)
            .Select(e => e.ToDto())
            .ToListAsync(ct);
        
        var virtualChannels = await this._context.Channels.OfType<VirtualInput>()
            .Include(e => e.Alert)
            .ThenInclude(e => ((DiscreteAlert)e).AlertLevel)
            .ThenInclude(e => e.DeviceAction)
            .Where(e => e.ModbusDeviceId == req.Id)
            .Select(e => e.ToDto())
            .ToListAsync(ct);

        var outputs = await this._context.Channels.OfType<DiscreteOutput>()
            .Where(e => e.ModbusDeviceId == req.Id)
            .Select(e => e.ToDto())
            .ToListAsync(ct);

        var response = new GetDeviceChannelsResponse() {
            AnalogInputs = analogChannels.AsEnumerable(),
            DiscreteInputs = discreteChannels.AsEnumerable(),
            VirtualInputs = virtualChannels.AsEnumerable(),
            DiscreteOutputs = outputs
        };
        await SendOkAsync(response, ct);
    }
}