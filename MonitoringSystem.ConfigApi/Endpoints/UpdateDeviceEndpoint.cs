using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Contracts.Requests.Update;
using MonitoringSystem.ConfigApi.Contracts.Responses.Update;
using MonitoringSystem.ConfigApi.Mapping;

namespace MonitoringSystem.ConfigApi.Endpoints; 

[HttpPut("devices/ModbusDevice"),AllowAnonymous]
public class UpdateDeviceEndpoint:Endpoint<UpdateDeviceRequest,UpdateDeviceResponse> {
    private readonly MonitorContext _context;

    public UpdateDeviceEndpoint(MonitorContext context) {
        this._context = context;
    }
    
    public override async Task HandleAsync(UpdateDeviceRequest req, CancellationToken ct) {
        var device = req.ModbusDevice.ToEntity();
        this._context.Update(device);
        var ret = await this._context.SaveChangesAsync(ct);
        if (ret > 0) {
            var updated = await this._context.Devices.OfType<ModbusDevice>()
                .Include(e => e.ModbusConfiguration)
                .Include(e => e.NetworkConfiguration)
                .Include(e => e.ChannelRegisterMap)
                .FirstOrDefaultAsync(e => e.Id == device.Id,ct);
            if (updated is not null) {
                UpdateDeviceResponse response = new UpdateDeviceResponse() {
                    ModbusDevice = updated.ToDto()
                };
                await SendOkAsync(response, ct);
            } else {
                await SendNotFoundAsync(ct);
            }
        } else {
            await SendNotFoundAsync(ct);
        }
    }
}