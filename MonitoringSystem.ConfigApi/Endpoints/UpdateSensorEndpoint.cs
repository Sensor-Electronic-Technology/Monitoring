using MonitoringConfig.Data.Model;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using MonitoringSystem.ConfigApi.Mapping;
using MonitoringSystem.Shared.Contracts.Requests.Update;
using MonitoringSystem.Shared.Contracts.Responses.Update;

namespace MonitoringSystem.ConfigApi.Endpoints; 

[HttpPut("sensors/Sensor"),AllowAnonymous]
public class UpdateSensorEndpoint:Endpoint<UpdateSensorRequest,UpdateSensorResponse> {
    private readonly MonitorContext _context;
    public UpdateSensorEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(UpdateSensorRequest req, CancellationToken ct) {
        var sensorEntity = req.Sensor.ToEntity();
        var updated = this._context.Update(sensorEntity).Entity.ToDto();
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new UpdateSensorResponse() { Sensor = updated }, ct);
        } else {
            await SendErrorsAsync(400, ct);
        }
    }
}