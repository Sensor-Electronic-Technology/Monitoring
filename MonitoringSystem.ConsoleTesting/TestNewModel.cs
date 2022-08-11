using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringData.Infrastructure.Data;
using MonitoringSystem.Shared.Data;

namespace MonitoringSystem.ConsoleTesting;


public class TestIdClass {
    public ObjectId _id { get; set; }
    public string Name { get; set; }
}

public class TestNewModel {
    static async Task Main(string[] args) {
        //await CreateNewMongoDb("epi1");
        //await CreateFacilityActions();
        //await CreateNewMongoDb();
        await CreateAnalog();
    }

    static async Task TestIdGeneration() {
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("test_types");
        var collection = database.GetCollection<TestIdClass>("test_ids");
        TestIdClass test1 = new TestIdClass();
        test1._id=ObjectId.GenerateNewId();
        test1.Name = "test1";
        Console.WriteLine($"Test1._id: {test1._id}");
        TestIdClass test2 = new TestIdClass();
        test2._id=ObjectId.GenerateNewId();
        test2.Name = "test2";
        Console.WriteLine($"Test2._id: {test2._id}");
        await collection.InsertOneAsync(test1);
        await collection.InsertOneAsync(test2);
        Console.WriteLine();
        Console.WriteLine();
        var tests = await collection.Find(_ => true).ToListAsync();
        foreach (var item in tests) {
            Console.WriteLine($"{item.Name}._id: {item._id}");
        }
        Console.WriteLine("Done");
    }
    
    static async Task CreateFacilityActions() {
        using var context = new FacilityContext();
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("epi1_data_dev");
        var outputCollection = database.GetCollection<ModuleOutputChannel>("output_items");
        var actionCollection = database.GetCollection<MonitorAction>("action_items");
        
        var device = context.Devices.OfType<MonitoringBox>()
            .AsNoTracking()
            .Include(e=>e.ModbusActionMapping)
            .ThenInclude(e=>e.FacilityAction)
            .FirstOrDefault(e => e.Identifier == "epi1");
        
        var discreteOutputs = await context.Channels.OfType<DiscreteOutput>()
            .AsNoTracking()
            .Include(e=>e.ModbusDevice)
            .Where(e => e.ModbusDevice.Identifier == device.Identifier)
            .OrderBy(e=>e.SystemChannel)
            .ToListAsync();
        
        List<ModuleOutputChannel> moduleOutputChannels = new List<ModuleOutputChannel>();
        foreach (var channel in discreteOutputs) {
            var outputChannel = new ModuleOutputChannel() {
                _id=ObjectId.GenerateNewId(),
                ChannelAddress = new ModuleAddress(channel.ChannelAddress.Channel, channel.ChannelAddress.ModuleSlot),
                ChannelName = channel.Identifier,
                SystemChannel = channel.SystemChannel,
                Connected = channel.Connected,
                Display = channel.Display,
                DisplayName = channel.DisplayName,
                ModbusAddress = channel.ModbusAddress
            };
            moduleOutputChannels.Add(outputChannel);
        }
        
        MonitorAction okay = new MonitorAction();
        okay._id = ObjectId.GenerateNewId();
        okay.Name = "Okay";
        okay.ActionType = ActionType.Okay;
        okay.FirmwareId = 0;
        okay.EmailEnabled = false;
        okay.EmailPeriod = 0;
        okay.ActionOutputs = new List<MonitorActionOutput>() {
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 4)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.High
            },
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 1)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.Low
            },
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 2)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.Low
            }
        };
        
        MonitorAction alarm = new MonitorAction();
        alarm._id = ObjectId.GenerateNewId();
        alarm.Name = "Alarm";
        alarm.ActionType = ActionType.Alarm;
        alarm.FirmwareId = 1;
        alarm.EmailEnabled = true;
        alarm.EmailPeriod = 30;
        alarm.ActionOutputs = new List<MonitorActionOutput>() {
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 1)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.High
            },
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 4)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.Low
            },
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 2)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.Low
            }
        };
        
        MonitorAction warning = new MonitorAction();
        warning._id = ObjectId.GenerateNewId();
        warning.Name = "Warning";
        warning.ActionType = ActionType.Warning;
        warning.FirmwareId = 2;
        warning.EmailEnabled = true;
        warning.EmailPeriod = 60;
        warning.ActionOutputs = new List<MonitorActionOutput>() {
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 2)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.High
            },
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 1)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.Low
            },
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 4)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.Low
            }
        };
        
        MonitorAction softwarn = new MonitorAction();
        softwarn._id = ObjectId.GenerateNewId();
        softwarn.Name = "SoftWarn";
        softwarn.ActionType = ActionType.SoftWarn;
        softwarn.FirmwareId = 3;
        softwarn.EmailEnabled = true;
        softwarn.EmailPeriod = 120;

        
        
        MonitorAction maint = new MonitorAction();
        maint._id = ObjectId.GenerateNewId();
        maint.Name = "Maintenance";
        maint.ActionType = ActionType.Maintenance;
        maint.FirmwareId = 4;
        maint.EmailEnabled = false;
        maint.EmailPeriod = 0;
        maint.ActionOutputs = new List<MonitorActionOutput>() {
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 4)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.High
            },
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 2)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.High
            },
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 1)._id,
                OffLevel = DiscreteState.Low,
                OnLevel = DiscreteState.Low
            }
        };
        
        MonitorAction reset = new MonitorAction();
        reset._id = ObjectId.GenerateNewId();
        reset.Name = "Reset Water Sensor";
        reset.ActionType = ActionType.Custom;
        reset.FirmwareId = 5;
        reset.EmailEnabled = false;
        reset.EmailPeriod = 0;
        reset.ActionOutputs = new List<MonitorActionOutput>() {
            new MonitorActionOutput() {
                DiscreteOutputId = moduleOutputChannels.FirstOrDefault(e => e.SystemChannel == 6)._id,
                OffLevel = DiscreteState.High,
                OnLevel = DiscreteState.Low
            },
        };

        await outputCollection.InsertManyAsync(moduleOutputChannels);
        await actionCollection.InsertOneAsync(okay);
        await actionCollection.InsertOneAsync(alarm);
        await actionCollection.InsertOneAsync(warning);
        await actionCollection.InsertOneAsync(maint);
        await actionCollection.InsertOneAsync(softwarn);
        await actionCollection.InsertOneAsync(reset);
        Console.WriteLine("Check Database");

    }
    
    static async Task<List<SensorTypeDev>> CreateNewSensor() {
        var context = new FacilityContext();
        var sensors=await context.Sensors.ToListAsync();
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("monitor_settings");
        var collection = database.GetCollection<SensorTypeDev>("sensors_dev");
        
        var sensorTypes=sensors.Select(sensor=>new SensorTypeDev() {
            _id = ObjectId.GenerateNewId(),
            Name = sensor.Name,
            Slope  = sensor.Slope,
            Offset = sensor.Offset,
            YAxisStart = sensor.YAxisMin,
            YAxisStop = sensor.YAxitMax,
            Units = sensor.Units
        });

        await collection.InsertManyAsync(sensorTypes);
        Console.WriteLine("Sensors Created");
        return sensorTypes.ToList();
    }

     public static async Task CreateNewMongoDb() {
        using var context = new FacilityContext();
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("epi1_data_dev");
        var device = context.Devices.OfType<MonitoringBox>()
            .AsNoTracking()
            .Include(e=>e.ModbusActionMapping)
            .ThenInclude(e=>e.FacilityAction)
            .FirstOrDefault(e => e.Identifier == "epi1");
        
        Console.WriteLine("Creating Collections");
        
        var discreteCollection = database.GetCollection<ModuleDiscreteChannel>("discrete_items");
        var alertCollection = database.GetCollection<ChannelAlert>("alert_items");
        /*var analogCollection = database.GetCollection<ModuleAnalogChannel>("analog_items");
        var virtualCollection = database.GetCollection<ModuleVirtualChannel>("virtual_items");
        var outputCollection = database.GetCollection<ModuleOutputChannel>("output_items");*/
        
        var actionCollection = database.GetCollection<MonitorAction>("action_items");
        var monitorActions = await actionCollection.Find(_ => true).ToListAsync();

        var oldAlerts = await context.Alerts.OfType<DiscreteAlert>()
            .Include(e => e.InputChannel)
                .ThenInclude(e => e.ModbusDevice)
            .Include(e => e.AlertLevel)
                .ThenInclude(e => e.FacilityAction)
            .Where(e => e.InputChannel is DiscreteInput && e.InputChannel.ModbusDevice.Identifier==device.Identifier)
            .ToListAsync();
        
        /*var discreteChannels = await context.Channels.OfType<DiscreteInput>()
            .AsNoTracking()
            .Include(e=>e.ModbusDevice)
            .Include(e=>e.Alert)
                .ThenInclude(e=>((DiscreteAlert)e).AlertLevel)
                    .ThenInclude(e=>e.FacilityAction)
            .Where(e => e.ModbusDevice.Identifier == device.Identifier)
            .OrderBy(e=>e.SystemChannel)
            .ToListAsync();*/
        
        List<ModuleDiscreteChannel> moduleDiscreteChannels = new List<ModuleDiscreteChannel>();
        List<MonitorDiscreteAlert> alerts = new List<MonitorDiscreteAlert>();
        foreach (var oldAlert in oldAlerts ) {
            var channel = oldAlert.InputChannel as DiscreteInput;
            var discreteChannel=new ModuleDiscreteChannel() {
                _id=ObjectId.GenerateNewId(),
                ChannelAddress = new ModuleAddress(channel.ChannelAddress.Channel, channel.ChannelAddress.ModuleSlot),
                ChannelName = channel.Identifier,
                SystemChannel = channel.SystemChannel,
                Connected = channel.Connected,
                Display = channel.Display,
                DisplayName = channel.DisplayName,
                ModbusAddress = channel.ModbusAddress
            };
            MonitorDiscreteAlert alert = new MonitorDiscreteAlert();
            alert._id = ObjectId.GenerateNewId();
            alert.Bypass = oldAlert.Bypass;
            alert.Enabled = oldAlert.Enabled;
            alert.ItemId = discreteChannel._id;
            alert.Name = oldAlert.DisplayName;
            alert.ModbusAddress = oldAlert.ModbusAddress;
            alert.BypassResetTime = oldAlert.BypassResetTime;
            if (oldAlert.AlertLevel != null) {
                MonitorDiscretLevel level = new MonitorDiscretLevel();
                level.TriggerOn = oldAlert.AlertLevel.TriggerOn;
                level.Enabled = oldAlert.AlertLevel.Enabled;
                level.FacilityActionId =
                    monitorActions.FirstOrDefault(e => e.ActionType == oldAlert.AlertLevel.FacilityAction.ActionType)._id;
                alert.AlertLevel = level;
            }
            discreteChannel.AlertId = alert._id;
            moduleDiscreteChannels.Add(discreteChannel);
            alerts.Add(alert);
        }
        
        await discreteCollection.InsertManyAsync(moduleDiscreteChannels);
        await alertCollection.InsertManyAsync(alerts);
        Console.WriteLine("Check Database");


        /*var analogChannels = await context.Channels.OfType<AnalogInput>()
            .AsNoTracking()
            .Include(e=>e.ModbusDevice)
            .Where(e => e.ModbusDevice.Identifier == device.Identifier)
            .OrderBy(e=>e.SystemChannel)
            .ToListAsync();
        
        List<ModuleAnalogChannel> moduleAnalogChannels = new List<ModuleAnalogChannel>();
        foreach (var channel in analogChannels) {
            var analogChannel = new ModuleAnalogChannel() {
                ChannelAddress = new ModuleAddress(channel.ChannelAddress.Channel, channel.ChannelAddress.ModuleSlot),
                ChannelName = channel.Identifier,
                SystemChannel = channel.SystemChannel,
                Connected = channel.Connected,
                Display = channel.Display,
                DisplayName = channel.DisplayName,
                ModbusAddress = channel.ModbusAddress
            };
            moduleAnalogChannels.Add(analogChannel);
        }
        
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
            });*/

        /*device.ModbusActionMapping.Select(actionMap => new MonitorAction() {
            Name = actionMap.FacilityAction.ActionName,
            ActionType = actionMap.FacilityAction.ActionType,
            EmailEnabled = actionMap.FacilityAction.EmailEnabled,
            EmailPeriod = actionMap.FacilityAction.EmailPeriod,
            ActionOutputs = actionMap.FacilityAction.ActionOutputs.Select(actionOutput=>new MonitorActionOutput() {
                OnLevel = actionOutput.OnLevel,
                OffLevel = actionOutput.OffLevel
            }).ToList()
        });*/


        /*IMongoCollection<ModuleAnalogChannel> analogItems = database.GetCollection<ModuleAnalogChannel>("analog_items");
        var analogChannels = await context.Channels.OfType<AnalogInput>()
            .AsNoTracking()
            .Include(e=>e.ModbusDevice)
            .Where(e => e.ModbusDevice.Identifier == device.Identifier)
            .OrderBy(e=>e.SystemChannel)
            .Select(channel=>new ModuleAnalogChannel() { 
                ChannelName =channel.Identifier,
                DisplayName = channel.DisplayName
            }).ToListAsync();*/
     }
     
     public static async Task CreateAnalog() {
        using var context = new FacilityContext();
        var client = new MongoClient("mongodb://172.20.3.41");
        var database = client.GetDatabase("epi1_data_dev");
        var device = context.Devices.OfType<MonitoringBox>()
            .AsNoTracking()
            .Include(e=>e.ModbusActionMapping)
            .ThenInclude(e=>e.FacilityAction)
            .FirstOrDefault(e => e.Identifier == "epi1");
        
        Console.WriteLine("Creating Collections");
        
        var analogCollection = database.GetCollection<ModuleAnalogChannel>("analog_items");
        var alertCollection = database.GetCollection<ChannelAlert>("alert_items");
        
        var actionCollection = database.GetCollection<MonitorAction>("action_items");
        var monitorActions = await actionCollection.Find(_ => true).ToListAsync();

        var oldAlerts = await context.Alerts.OfType<AnalogAlert>()
            .Include(e => e.InputChannel)
                .ThenInclude(e => e.ModbusDevice)
            .Include(e => e.AlertLevels)
                .ThenInclude(e => e.FacilityAction)
            .Where(e => e.InputChannel is AnalogInput && e.InputChannel.ModbusDevice.Identifier==device.Identifier)
            .ToListAsync();
        
        List<ModuleAnalogChannel> moduleAnalogChannels = new List<ModuleAnalogChannel>();
        List<MonitorAnalogAlert> alerts = new List<MonitorAnalogAlert>();
        foreach (var oldAlert in oldAlerts ) {
            var channel = oldAlert.InputChannel as AnalogInput;
            var analogChannel=new ModuleAnalogChannel() {
                _id=ObjectId.GenerateNewId(),
                ChannelAddress = new ModuleAddress(channel.ChannelAddress.Channel, channel.ChannelAddress.ModuleSlot),
                ChannelName = channel.Identifier,
                SystemChannel = channel.SystemChannel,
                Connected = channel.Connected,
                Display = channel.Display,
                DisplayName = channel.DisplayName,
                ModbusAddress = channel.ModbusAddress
            };
            MonitorAnalogAlert alert = new MonitorAnalogAlert();
            alert._id = ObjectId.GenerateNewId();
            alert.Bypass = oldAlert.Bypass;
            alert.Enabled = oldAlert.Enabled;
            alert.ItemId = analogChannel._id;
            alert.Name = oldAlert.DisplayName;
            alert.ModbusAddress = oldAlert.ModbusAddress;
            alert.BypassResetTime = oldAlert.BypassResetTime;
            if (oldAlert.AlertLevels.Any()) {
                alert.AlertLevels = new List<MonitorAnalogLevel>();
                foreach (var oldlevel in oldAlert.AlertLevels) {
                    MonitorAnalogLevel level = new MonitorAnalogLevel();
                    level.Enabled = oldlevel.Enabled;
                    level.SetPoint = oldlevel.SetPoint;
                    if (oldlevel.FacilityAction != null) {
                        level.FacilityActionId = monitorActions
                            .FirstOrDefault(e => e.ActionType == oldlevel.FacilityAction.ActionType)._id;
                    }
                    alert.AlertLevels.Add(level);
                }
            }
            analogChannel.AlertId = alert._id;
            moduleAnalogChannels.Add(analogChannel);
            alerts.Add(alert);
        }
        
        await analogCollection.InsertManyAsync(moduleAnalogChannels);
        await alertCollection.InsertManyAsync(alerts);
        Console.WriteLine("Check Database");
     }
}