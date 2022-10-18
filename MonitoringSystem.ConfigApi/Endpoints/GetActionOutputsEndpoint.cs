using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Mapping;
using MonitoringSystem.Shared.Contracts.Requests.Get;
using MonitoringSystem.Shared.Contracts.Responses.Get;

namespace MonitoringSystem.ConfigApi.Endpoints; 
[HttpGet("actionoutputs/"),AllowAnonymous]
public class GetActionOutputsEndpoint:Endpoint<GetActionOutputsRequest,GetActionOutputsResponse> {
    private readonly MonitorContext _context;

    public GetActionOutputsEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(GetActionOutputsRequest req, CancellationToken ct) {
        var actionOutputs = await this._context.ActionOutputs
            .Where(e => e.DeviceActionId == req.DeviceActionId)
            .Select(e=>e.ToDto())
            .ToListAsync(ct);
        if (actionOutputs.Any()) {
            var response = new GetActionOutputsResponse() { ActionOutputs = actionOutputs };
            await SendOkAsync(response, ct);
        } else {
            await SendNotFoundAsync(ct);
        }
    }
}