using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Contracts.Responses.Get;
using MonitoringSystem.ConfigApi.Mapping;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Endpoints;

[HttpGet("devices"),AllowAnonymous]
public class GetAllDevicesEndpoint:EndpointWithoutRequest<List<ModbusDeviceDto>> {
    private readonly MonitorContext _context;

    public GetAllDevicesEndpoint(MonitorContext context) {
        this._context = context;
    }

    public override async Task HandleAsync(CancellationToken ct) {
        var devices = await this._context.Devices.OfType<ModbusDevice>()
            .Include(e=>e.ModbusConfiguration)
            .Include(e=>e.NetworkConfiguration)
            .Include(e=>e.ChannelRegisterMap)
            .ToListAsync(ct);
        var response = devices.ToDeviceResponse();
        await SendOkAsync(response.Devices.ToList(), ct);
    }
}