using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MonitoringConfig.Data.Model;
using MonitoringSystem.ConfigApi.Mapping;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringSystem.ConsoleTesting; 

public class CloneDatabase {

    static async Task Main(string[] args) {
        //await CreateManagedDevice("nh3");
        //await CreateMongoDB("nh3");
        //await UpdateAlerts("gasbay");
        /*await UpdateAlertNames("epi1");
        await UpdateAlertNames("epi2");
        await UpdateAlertNames("gasbay");*/
        /*await UpdateChannels("epi1");
        await UpdateChannels("epi2");
        await UpdateChannels("gasbay");
        await UpdateChannels("nh3");*/
    }

    static async Task UpdateAlertNames(string deviceName) {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase($"{deviceName}_data_dev");
        var alertCollection = database.GetCollection<MonitorAlert>("alert_items");
        var alerts = await alertCollection.FindSync(_ => true).ToListAsync();
        foreach (var alert in alerts) {
            var filter = Builders<MonitorAlert>.Filter.Eq(e => e._id, alert._id);
            var index = alert.DisplayName.IndexOf("Alert");
            if (index > 0) {
                var newName = alert.DisplayName.Remove(index);
                var update = Builders<MonitorAlert>.Update.Set(e => e.DisplayName, newName.TrimEnd());
                await alertCollection.UpdateOneAsync(filter, update);
            }

        }
        Console.WriteLine($"Check {deviceName} database");
    }

    static async Task UpdateChannels(string deviceName) {
        await using var context = new MonitorContext();
        var inputs =await context.Channels
            .OfType<InputChannel>()
            .Include(e => e.ModbusDevice)
            .Include(e => e.Alert)
            .Where(e => e.ModbusDevice.Name == deviceName.ToLower())
            .ToListAsync();
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase($"{deviceName}_data_dev");
        var discreteCollection = database.GetCollection<DiscreteItem>("discrete_items");
        var analogCollection = database.GetCollection<AnalogItem>("analog_items");
        var virtualCollection = database.GetCollection<VirtualItem>("virtual_items");
        foreach (var input in inputs) {
            input.Display = input.Alert.Enabled;
            if (typeof(AnalogInput) == input.GetType()) {
                var filter = Builders<AnalogItem>.Filter.Eq(e => e.ItemId, input.Id.ToString());
                var update = Builders<AnalogItem>.Update
                    .Set(e => e.Display, input.Display);
                await analogCollection.UpdateOneAsync(filter, update);
            }else if (typeof(DiscreteInput) == input.GetType()) {
                var filter = Builders<DiscreteItem>.Filter.Eq(e => e.ItemId, input.Id.ToString());
                var update = Builders<DiscreteItem>.Update
                    .Set(e => e.Display, input.Display);
                await discreteCollection.UpdateOneAsync(filter, update);
            }else if (typeof(VirtualInput) == input.GetType()) {
                var filter = Builders<VirtualItem>.Filter.Eq(e => e.ItemId, input.Id.ToString());
                var update = Builders<VirtualItem>.Update
                    .Set(e => e.Display, input.Display);
                await virtualCollection.UpdateOneAsync(filter, update);
            }
        }
        context.UpdateRange(inputs);
        await context.SaveChangesAsync();
        Console.WriteLine($"Check {deviceName} database");
    }

    static async Task UpdateAlertsMongo(string deviceName) {
        await using var context = new MonitorContext();
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase($"{deviceName}_data_dev");
        var alertCollection = database.GetCollection<MonitorAlert>("alert_items");
        
        var inputs =await context.Channels
            .OfType<InputChannel>()
            .Include(e => e.ModbusDevice)
            .Include(e => e.Alert)
            .Where(e => e.ModbusDevice.Name == deviceName.ToLower())
            .ToListAsync();
       
        foreach (var input in inputs) {
            AlertItemType itemType = AlertItemType.Discrete;
            if (typeof(AnalogInput) == input.GetType()) {
                itemType = AlertItemType.Analog;
            }else if (typeof(DiscreteInput) == input.GetType()) {
                itemType = AlertItemType.Discrete;
            }else if (typeof(VirtualInput) == input.GetType()) {
                itemType = AlertItemType.Virtual;
            }

            var filter = Builders<MonitorAlert>.Filter.Eq(e => e.EntityId, input.Alert?.Id.ToString());
            var update = Builders<MonitorAlert>.Update
                .Set(e => e.Enabled, input.Alert?.Enabled)
                .Set(e=>e.AlertItemType,itemType);
            await alertCollection.UpdateOneAsync(filter, update);
        }
        Console.WriteLine("Check Database");
    }
    
    
    public static async Task CreateMongoDB(string deviceName) {
            using var context = new MonitorContext();
            var client = new MongoClient("mongodb://172.20.3.41");
            var sensorTypes = client.GetDatabase("monitor_settings_dev")
                .GetCollection<SensorType>("sensor_types");
            
            var sensors = await sensorTypes.Find(_ => true).ToListAsync();
            var managedCollection = client.GetDatabase("monitor_settings_dev")
                .GetCollection<ManagedDevice>("monitor_devices");
            
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .Include(e=>e.DeviceActions)
                .ThenInclude(e=>e.FacilityAction)
                .FirstOrDefault(e => e.Name == deviceName);
            
            var managedDevice = await managedCollection.Find(e => e.DeviceName == device.Name)
                .FirstOrDefaultAsync();
            
            if(device is not null) {
                Console.WriteLine($"Device {device.Name} found");
                var database = client.GetDatabase($"{device.Name.ToLower()}_data_dev");
                Console.WriteLine("Creating Collections");
                Console.WriteLine($"Device {device.Name.ToLower()} found");

                Console.WriteLine("Creating Collections");
                
                await database.CreateCollectionAsync("analog_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("discrete_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp",granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("virtual_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", granularity: TimeSeriesGranularity.Seconds)
                    });
                
                await database.CreateCollectionAsync("alert_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp",granularity: TimeSeriesGranularity.Seconds)
                    });

                Console.WriteLine("Collections Created, Populating configurations");
                var analogCollection = database.GetCollection<AnalogItem>("analog_items");
                var alertCollection = database.GetCollection<MonitorAlert>("alert_items");
                var discreteCollection = database.GetCollection<DiscreteItem>("discrete_items");
                var virtualCollection = database.GetCollection<VirtualItem>("virtual_items");
                var actionCollection = database.GetCollection<ActionItem>("action_items");
                
                var virtualItems = new List<VirtualItem>();
                var analogItems = new List<AnalogItem>();
                var alertItems = new List<MonitorAlert>();
                
                var analogInputs = await context.Channels.OfType<AnalogInput>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Include(e => e.Alert)
                    .ThenInclude(e=>((AnalogAlert)e).AlertLevels)
                    .ThenInclude(e=>e.DeviceAction.FacilityAction)
                    .Include(e=>e.Sensor)
                    .Where(e => e.ModbusDevice.Name == device.Name)
                    .OrderBy(e => e.SystemChannel)
                    .ToListAsync();
                
                foreach (var input in analogInputs) {
                    var sensor = sensors.Find(e => e.EntityId == input.SensorId.ToString());
                    var analogItem = new AnalogItem {
                        _id = ObjectId.GenerateNewId(),
                        Identifier = input.DisplayName,
                        SystemChannel = input.SystemChannel,
                        ItemId = input.Id.ToString(),
                        Factor = 10,
                        Display = input.Display,
                        ManagedDeviceId = managedDevice._id,
                        SensorId = sensor?._id ?? ObjectId.Empty,
                        RecordThreshold = input.Sensor!=null ? (float)input.Sensor.RecordThreshold:0.00f,
                        Level1Action = ActionType.SoftWarn,
                        Level1SetPoint = 0,
                        Level2Action = ActionType.Warning,
                        Level2SetPoint = 0,
                        Level3Action = ActionType.Alarm,
                        Level3SetPoint = 0,
                        Register = input.ModbusAddress.Address,
                        RegisterLength = input.ModbusAddress.RegisterLength
                    };
                    var analogAlert = input.Alert as AnalogAlert;
                    var alertLevels = analogAlert.AlertLevels.ToArray();
                    analogItem.Level1Action = alertLevels[0].DeviceAction.FacilityAction.ActionType;
                    analogItem.Level1SetPoint = (float)alertLevels[0].SetPoint;
                    analogItem.Level2Action = alertLevels[1].DeviceAction.FacilityAction.ActionType;
                    analogItem.Level2SetPoint = (float)alertLevels[1].SetPoint;
                    analogItem.Level3Action = alertLevels[2].DeviceAction.FacilityAction.ActionType;
                    analogItem.Level3SetPoint = (float)alertLevels[2].SetPoint;
                    analogItems.Add(analogItem);
                    alertItems.Add(new MonitorAlert() {
                        _id = ObjectId.GenerateNewId(),
                        EntityId=input.Alert.Id.ToString(),
                        AlertItemType = AlertItemType.Analog,
                        Bypassed = input.Alert.Bypass,
                        BypassResetTime = input.Alert.BypassResetTime,
                        DisplayName = input.Alert.Name,
                        Enabled = input.Alert.Enabled,
                        ChannelId = analogItem._id
                    });
                }

                
                /*var discreteItems = new List<DiscreteItem>();
                var discreteInputs = await context.Channels.OfType<DiscreteInput>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Include(e => e.Alert)
                    .ThenInclude(e => ((DiscreteAlert)e).AlertLevel)
                    .ThenInclude(e => e.DeviceAction.FacilityAction)
                    .Where(e => e.ModbusDevice.Name == device.Name)
                    .OrderBy(e => e.SystemChannel).
                    ToListAsync();

                foreach (var input in discreteInputs) {
                    var discreteItem= new DiscreteItem() {
                        _id = ObjectId.GenerateNewId(),
                        Display = input.Display,
                        Identifier = input.Identifier ?? string.Empty,
                        ManagedDeviceId = managedDevice._id,
                        ItemId = input.Id.ToString(),
                        SystemChannel = input.SystemChannel,
                    };
                    if (input.Alert is DiscreteAlert discreteAlert) {
                        if (discreteAlert.AlertLevel.DeviceAction != null) {
                            discreteItem.ActionType= discreteAlert.AlertLevel.DeviceAction.FacilityAction.ActionType;
                            discreteItem.TriggerOn = discreteAlert.AlertLevel.TriggerOn;
                        }
                    }
                    discreteItems.Add(discreteItem);
                    alertItems.Add(new MonitorAlert() {
                        _id = ObjectId.GenerateNewId(),
                        EntityId=input.Alert.Id.ToString(),
                        AlertItemType = AlertItemType.Discrete,
                        Bypassed = input.Alert.Bypass,
                        BypassResetTime = input.Alert.BypassResetTime,
                        DisplayName = input.Alert.Name,
                        Enabled = input.Alert.Enabled,
                        ChannelId=discreteItem._id
                    });
                    
                }*/
                
                /*var virtualInputs = await context.Channels.OfType<VirtualInput>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Include(e => e.Alert)
                    .ThenInclude(e => ((DiscreteAlert)e).AlertLevel)
                    .ThenInclude(e => e.DeviceAction.FacilityAction)
                    .Where(e => e.ModbusDevice.Name == device.Name)
                    .OrderBy(e => e.SystemChannel).ToListAsync();

                foreach (var input in virtualInputs) {
                    var virtualItem = new VirtualItem() {
                        _id = ObjectId.GenerateNewId(),
                        Display = input.Display,
                        Identifier = input.Identifier,
                        ManagedDeviceId = managedDevice._id,
                        ItemId = input.Id.ToString(),
                        SystemChannel = input.SystemChannel,
                        Register = input.ModbusAddress.Address
                    };
                    if (input.Alert is DiscreteAlert discreteAlert) {
                        if (discreteAlert.AlertLevel.DeviceAction != null) {
                            virtualItem.ActionType= discreteAlert.AlertLevel.DeviceAction.FacilityAction.ActionType;
                            virtualItem.TriggerOn = discreteAlert.AlertLevel.TriggerOn;
                        }
                    }
                    virtualItems.Add(virtualItem);
                    alertItems.Add(new MonitorAlert() {
                        _id = ObjectId.GenerateNewId(),
                        EntityId=input.Alert.Id.ToString(),
                        AlertItemType = AlertItemType.Virtual,
                        Bypassed = input.Alert.Bypass,
                        BypassResetTime = input.Alert.BypassResetTime,
                        DisplayName = input.Alert.Name,
                        Enabled = input.Alert.Enabled,
                        ChannelId=virtualItem._id
                    });
                }*/
                
                var actionItems = device.DeviceActions.Select(action => new ActionItem() {
                    _id=ObjectId.GenerateNewId(),
                    ActionType = action.FacilityAction?.ActionType ?? ActionType.Okay,
                    Display = true,
                    EmailEnabled = action.FacilityAction?.EmailEnabled ?? false,
                    EmailPeriod = action.FacilityAction?.EmailPeriod ?? 0,
                    Identifier = action.Name ?? string.Empty,
                    ItemId=action.Id.ToString(),
                    ManagedDeviceId = managedDevice._id
                }).ToList();
                
                await analogCollection.InsertManyAsync(analogItems);
                /*await discreteCollection.InsertManyAsync(discreteItems);
                await virtualCollection.InsertManyAsync(virtualItems);*/
                await alertCollection.InsertManyAsync(alertItems);
                await actionCollection.InsertManyAsync(actionItems);
                
                Console.WriteLine("Check database, press any key to exit");
            } else {
                Console.WriteLine("Error: Device not found");
            }
            Console.ReadKey();
        }

    public static async Task CreateManagedDevice(string deviceName) {
        var context = new MonitorContext();
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("monitor_settings_dev");
        var sensorCollection = database.GetCollection<SensorType>("sensor_types");
        var managedCollection = database.GetCollection<ManagedDevice>("monitor_devices");
        var tSensor = await sensorCollection.Find(e => e.Name == "NH3 Tank Temp.").FirstOrDefaultAsync();;
        var dSensor =await  sensorCollection.Find(e => e.Name == "Duty Cycle").FirstOrDefaultAsync();;
        var wSensor =await sensorCollection.Find(e => e.Name == "Weight").FirstOrDefaultAsync();
        
        var device = await context.Devices.OfType<ModbusDevice>()
            .Include(e => e.NetworkConfiguration)
            .Include(e => e.ModbusConfiguration)
            .Include(e => e.ChannelRegisterMap)
            .Include(e => e.Channels)
                .ThenInclude(e=>((AnalogInput)e).Alert)
                    .ThenInclude(e=>((AnalogAlert)e).AlertLevels)
            .FirstOrDefaultAsync(e => e.Name == deviceName);
        
        if (device != null) {
            var managedDevice = new ManagedDevice {
                _id = ObjectId.GenerateNewId(),
                DeviceId = device.Id.ToString(),
                DatabaseName = device.Database,
                HubName = device.HubName,
                HubAddress = device.HubAddress,
                DeviceType = nameof(ModbusDevice),
                IpAddress = device.NetworkConfiguration.IpAddress,
                Port = device.NetworkConfiguration.Port,
                RecordInterval = device.ReadInterval,
                SensorTypes = new List<ObjectId>() {
                    tSensor._id,
                    wSensor._id,
                    dSensor._id
                },
                CollectionNames = new Dictionary<string, string>() {
                    [nameof(AnalogItem)]="analog_items",
                    [nameof(DiscreteItem)]="discrete_items",
                    [nameof(VirtualItem)]="virtual_items",
                    [nameof(OutputItem)]="output_items",
                    [nameof(ActionItem)]="action_items",
                    [nameof(MonitorAlert)]="alert_items",
                    [nameof(AnalogReadings)]="analog_readings",
                    [nameof(DiscreteReadings)]="discrete_readings",
                    [nameof(VirtualReadings)]="virtual_readings",
                    [nameof(AlertReadings)]="alert_readings"
                }, //managedDevice.CollectionNames
                ChannelMapping = device.ChannelRegisterMap?.ToDto(),
                ModbusConfiguration = device.ModbusConfiguration?.ToDto()
            };
            await managedCollection.InsertOneAsync(managedDevice);
            Console.WriteLine("Check Database");
            /*managedDevice.ChannelMapping = new ChannelMappingConfigDto() {
                AnalogRegisterType = device.ChannelRegisterMap.AnalogRegisterType,
                AnalogStart = device.ChannelRegisterMap.AnalogStart,
            };*/

        }
    }
}

