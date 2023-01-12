using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.EventContracts.Events;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringSystem.ConfigApi.EventContracts.Handlers; 

public class DeviceUpdatedHandler:FastEventHandler<DeviceUpdatedEvent> {
    public override async Task HandleAsync(DeviceUpdatedEvent eventModel, CancellationToken ct) {
        using var scope = CreateScope();
        var context = scope.Resolve<MonitorContext>();
        var logger = scope.Resolve<ILogger<DeviceUpdatedHandler>>();
        var client = Resolve<IMongoClient>();
        var deviceCollection = client.GetDatabase("monitor_settings")
            .GetCollection<ManagedDevice>("monitor_devices");
        var deviceDto = eventModel.ModbusDevice;
        var device = await context.Devices.FirstOrDefaultAsync(e => e.Id == deviceDto.Id,ct);
        if (device != null) {
            var update = Builders<ManagedDevice>.Update
                .Set(e => e.DeviceName, deviceDto.Name)
                .Set(e => e.HubName, deviceDto.HubName)
                .Set(e => e.HubAddress, deviceDto.HubAddress)
                .Set(e => e.RecordInterval, deviceDto.SaveInterval)
                .Set(e => e.DeviceType, device.GetType().Name)
                .Set(e => e.IpAddress, deviceDto.NetworkConfig.IpAddress)
                .Set(e => e.Port, deviceDto.NetworkConfig.Port);
            var filter = Builders<ManagedDevice>.Filter.Eq(e => e.DeviceId, deviceDto.Id.ToString());
            var result=await deviceCollection.UpdateOneAsync(filter, update,cancellationToken:ct);
            if (result.IsAcknowledged) {
                logger.LogInformation("Device Updated");
            } else {
                logger.LogError("Device Update Failed");
            }
        } else {
            logger.LogError("Device not found");
        }
    }
}