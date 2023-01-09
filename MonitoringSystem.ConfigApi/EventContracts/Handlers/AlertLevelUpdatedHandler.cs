using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.EventContracts.Events;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;
namespace MonitoringSystem.ConfigApi.EventContracts.Handlers;

public class AnalogLevelUpdatedHandler : FastEventHandler<AnalogLevelUpdatedEvent> {
    public override async Task HandleAsync(AnalogLevelUpdatedEvent eventModel, CancellationToken ct) {
        using var scope = CreateScope();
        var context = scope.Resolve<MonitorContext>();
        //var context = Resolve<MonitorContext>();
        var client = Resolve<IMongoClient>();
        var deviceCollection =
            client.GetDatabase("monitor_settings").GetCollection<ManagedDevice>("monitor_devices");
        
        var analogLevelDto = eventModel.AnalogLevelDto;
        var analogLevel = await context.AlertLevels.OfType<AnalogLevel>()
            .Include(e=>e.AnalogAlert)
                .ThenInclude(e=>e.InputChannel)
            .Include(e=>e.DeviceAction)
            .FirstOrDefaultAsync(e=>e.Id==analogLevelDto.Id,ct);
        
        if (analogLevel != null) {
            var managedDevice = await deviceCollection
                .Find(e => e.DeviceId ==analogLevel!.AnalogAlert!.InputChannel!.ModbusDeviceId.ToString())
                .FirstOrDefaultAsync(ct);
            if (managedDevice != null) {
                var collection = client.GetDatabase(managedDevice.DatabaseName)
                    .GetCollection<MonitorAlert>("analog_items");
                var filter = Builders<AnalogItem>.Filter.Eq(e => e.ItemId, 
                    analogLevel!.AnalogAlert!.InputChannelId.ToString());
                /*var update=Builders<AnalogItem>.Update
                    .Set(e=>e.)*/
            } else {
                Console.WriteLine("ManagedDevice Not Found");
            }
        } else {
            Console.WriteLine("Analog Level Not Found");
        }
    }
}

