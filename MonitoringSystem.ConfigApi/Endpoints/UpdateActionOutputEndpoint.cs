using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Mapping;
using MonitoringSystem.Shared.Contracts.Requests.Update;
using MonitoringSystem.Shared.Contracts.Responses.Update;

namespace MonitoringSystem.ConfigApi.Endpoints; 
[HttpPut("actionoutputs/ActionOutput"),AllowAnonymous]
public class UpdateActionOutputEndpoint:Endpoint<UpdateActionOutputRequest,UpdateActionOutputResponse> {
    private readonly MonitorContext _context;

    public UpdateActionOutputEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateActionOutputRequest req, CancellationToken ct) {
        var actionOutput = req.ActionOutput.ToEntity();
        var updated=this._context.Update(actionOutput).Entity.ToDto();
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateActionOutputResponse() { ActionOutput = updated }, ct);
        } else {
            await SendErrorsAsync(400, ct);
        }
    }
}