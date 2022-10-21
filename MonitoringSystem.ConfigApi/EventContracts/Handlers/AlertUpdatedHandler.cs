using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.EventContracts.Events;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;
namespace MonitoringSystem.ConfigApi.EventContracts.Handlers;

public class AlertUpdatedHandler:FastEventHandler<AlertUpdatedEvent> {
    public override async Task HandleAsync(AlertUpdatedEvent eventModel, CancellationToken ct) {
        using var scope = CreateScope();
        var context = scope.Resolve<MonitorContext>();
        //var context = Resolve<MonitorContext>();
        var client = Resolve<IMongoClient>();
        var deviceCollection =
            client.GetDatabase("monitor_settings_dev").GetCollection<ManagedDevice>("monitor_devices");
        
        var alertDto = eventModel.Alert;
        var alert = await context.Alerts
            .Include(e => e.InputChannel.ModbusDevice)
            .Where(e => e.Id == alertDto.Id)
            .FirstOrDefaultAsync(ct);
        if (alert != null) {
            var managedDevice = await deviceCollection
                .Find(e => e.DeviceId == alert.InputChannel.ModbusDeviceId.ToString())
                .FirstOrDefaultAsync(ct);
            if (managedDevice != null) {
                var collection = client.GetDatabase(managedDevice.DatabaseName)
                    .GetCollection<MonitorAlert>("alert_items");
                var filter = Builders<MonitorAlert>.Filter.Eq(e => e.EntityId,alert.Id.ToString());
                var update = Builders<MonitorAlert>.Update
                    .Set(e => e.DisplayName, alert.Name)
                    .Set(e => e.BypassResetTime, alert.BypassResetTime)
                    .Set(e => e.Enabled, alert.Enabled)
                    .Set(e => e.Register, alert.ModbusAddress.Address)
                    .Set(e => e.Bypassed, alert.Bypass);
                await collection.UpdateOneAsync(filter, update, cancellationToken: ct);
            } else {
                Console.WriteLine("ManagedDevice Not Found");
                //this._logger.LogError("ManagedDevice Not Found");
            }
        } else {
            //this._logger.LogError("Alert Not Found!");
            Console.WriteLine("Alert Not Found");
        }
    }
}