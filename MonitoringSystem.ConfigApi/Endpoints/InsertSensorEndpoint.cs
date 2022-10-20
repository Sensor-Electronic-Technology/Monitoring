using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Mapping;
using MonitoringSystem.Shared.Contracts.Requests.Insert;
using MonitoringSystem.Shared.Contracts.Responses.Insert;

namespace MonitoringSystem.ConfigApi.Endpoints; 

[HttpPost("sensors/Sensor"),AllowAnonymous]
public class InsertSensorEndpoint:Endpoint<InsertSensorRequest,InsertSensorResponse> {
    private readonly MonitorContext _context;

    public InsertSensorEndpoint(MonitorContext context) {
        this._context = context;
    }
    
    public override async Task HandleAsync(InsertSensorRequest req, CancellationToken ct) {
        var sensor = req.Sensor.ToEntity();
        sensor.Id = Guid.NewGuid();
        var inserted = this._context.Sensors.Add(sensor).Entity.ToDto();
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            await SendOkAsync(new InsertSensorResponse() { Sensor = inserted },ct);
        } else {
            await SendErrorsAsync(400, ct);
        }
    }
}