using MonitoringConfig.Data.Model;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringSystem.ConfigApi.Contracts.Responses.Get;
using MonitoringSystem.ConfigApi.Mapping;

namespace MonitoringSystem.ConfigApi.Endpoints;


[HttpGet("sensors"),AllowAnonymous]
public class GetAllSensorsEndpoint : EndpointWithoutRequest<GetAllSensorsResponse> {
    private readonly MonitorContext _context;

    public GetAllSensorsEndpoint(MonitorContext context) {
        this._context = context;
    }
    
    public override async Task HandleAsync(CancellationToken ct) {
        var sensors = await this._context.Sensors
            .Select(e => e.ToDto())
            .ToListAsync(ct);
        await SendOkAsync(new GetAllSensorsResponse() { Sensors = sensors }, ct);
    }
}