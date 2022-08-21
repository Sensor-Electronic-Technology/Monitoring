using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using MonitoringData.DataApi.Contracts.Responses;
using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringData.DataApi.Endpoints; 

[HttpGet("managed_devices"),AllowAnonymous]
public class GetAllManagedDevicesEndpoint:EndpointWithoutRequest<GetAllManagedDevicesResponse> {
    private readonly IMongoClient _client;

    public GetAllManagedDevicesEndpoint(IMongoClient client) {
        this._client = client;
    }

    public override async Task HandleAsync(CancellationToken ct) {
        var database = this._client.GetDatabase("monitor_settings");
        var collection = database.GetCollection<ManagedDevice>("monitor_devices");
        var devices = await collection.Find(_ => true).ToListAsync(ct);
        var response = new GetAllManagedDevicesResponse() {
            ManagedDevices = devices.AsEnumerable()
        };
        await SendOkAsync(response, ct);
    }
}