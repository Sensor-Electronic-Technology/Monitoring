using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MonitoringConfig.Data.Model;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringSystem.Shared.Data.LogModel;

namespace MonitoringSystem.ConsoleTesting {
    public class TestingNH {
        public static async Task Main(string[] args) {

            //await CreateNHDevice();
            //await CreateDeviceActions();
            //await CreateNH3Channels();
            //await DeleteDevice();
            //await DeleteChannels();

            //await CreateTempMonitorDevice();
            //await CreateDeviceActions("th");
            //await CreateTHSensors();
            await CreateThChannels();
        }

        

        public static async Task CreateNH3Channels() {
            var context = new MonitorContext();
            var device = context.Devices.OfType<ModbusDevice>()
                .Include(e => e.Channels)
                .AsTracking()
                .FirstOrDefault(e => e.Name == "nh3");

            var deviceActions = context.DeviceActions
                .Include(e=>e.FacilityAction)
                .Where(e => e.ModbusDeviceId == device.Id).ToList();

            var weightSensor = await context.Sensors.FirstOrDefaultAsync(e => e.Name=="Weight");
            var tempSensor = await context.Sensors.FirstOrDefaultAsync(e=>e.Name=="NH3 Tank Temp.");
            var dutySensor = await context.Sensors.FirstOrDefaultAsync(e => e.Name == "Duty Cycle");
            var inputs=CreateChannels(device, deviceActions,weightSensor,tempSensor,dutySensor);
            context.AddRange(inputs);
            await context.SaveChangesAsync();
            Console.WriteLine("Check Database");
        }

        public static async Task CreateTHSensors() {
            var context = new MonitorContext();
            var device = context.Devices.OfType<ModbusDevice>()
                .Include(e => e.Channels)
                .AsTracking()
                .FirstOrDefault(e => e.Name == "nh3");
            
            /*Sensor tSensor = new Sensor() {
                Id = Guid.NewGuid(),
                Name = "AreaHumidity",
                DisplayName = "Temperature",
                Factor = 100,
                RecordThreshold = 30,
                Slope = 1,
                Offset = 1,
                ThresholdInterval = 30
            };*/
            
            Sensor tSensor = new Sensor() {
                Id = Guid.NewGuid(),
                Name = "AreaTemperature",
                DisplayName = "Temperature",
                Factor = 100,
                RecordThreshold = 30,
                Slope = 1,
                Offset = 1,
                ThresholdInterval = 30
            };

            await context.Sensors.AddAsync(tSensor);
            await context.SaveChangesAsync();
        }
        
        public static async Task CreateThChannels() {
            var context = new MonitorContext();
            var device = context.Devices.OfType<ModbusDevice>()
                .Include(e => e.Channels)
                .AsTracking()
                .FirstOrDefault(e => e.Name == "th");
                
            var deviceActions = context.DeviceActions
                .Include(e=>e.FacilityAction)
                .Where(e => e.ModbusDeviceId == device.Id).ToList();
            
            var hSensor = await context.Sensors.FirstOrDefaultAsync(e => e.Name=="AreaHumidity");
            var tSensor = await context.Sensors.FirstOrDefaultAsync(e=>e.Name=="AreaTemperature");
            var inputs=GenerateThChannels(device, deviceActions,tSensor,hSensor);
            await context.AddRangeAsync(inputs);
            await context.SaveChangesAsync();
            Console.WriteLine("Check Database");
        }

        public static IList<AnalogInput> CreateChannels(ModbusDevice device,List<DeviceAction> actions,Sensor wSensor,Sensor tSensor,Sensor dSensor) {
            var soft = actions.FirstOrDefault(e => e.FacilityAction is { ActionType: ActionType.SoftWarn });
            var warn = actions.FirstOrDefault(e => e.FacilityAction is { ActionType: ActionType.Warning });
            var alrm = actions.FirstOrDefault(e => e.FacilityAction is { ActionType: ActionType.Alarm });

            var tank1 = CreateAnalogInput(device, "Tank1", 1, soft, warn, alrm, "Tank1 Weight", 0, 2, true, 200, 150, 100,wSensor);
            var tank2 = CreateAnalogInput(device, "Tank2", 1, soft, warn, alrm, "Tank2 Weight",2, 2, true, 200, 150, 100,wSensor);
            var temp1 = CreateAnalogInput(device, "Temp1", 1, soft, warn, alrm, "Tank1 Temp.", 60, 1, false, 0, 0, 0,tSensor);
            var temp2 = CreateAnalogInput(device, "Temp2", 1, soft, warn, alrm, "Tank2 Temp.", 61, 1, false, 0, 0, 0,tSensor);
            var h1 = CreateAnalogInput(device, "Heater1", 1, soft, warn, alrm, "Heater1 Duty Cycle", 66, 1, false, 0, 0, 0,dSensor);
            var h2 = CreateAnalogInput(device, "Heater2", 1, soft, warn, alrm, "Heater1 Duty Cycle", 67, 1, false, 0, 0, 0,dSensor);
            return new List<AnalogInput>() { tank1, tank2, temp1, temp2, h1, h2 };
        }
        
        public static IList<AnalogInput> GenerateThChannels(ModbusDevice device,List<DeviceAction> actions,Sensor tSensor,Sensor hSensor) {
            var soft = actions.FirstOrDefault(e => e.FacilityAction is { ActionType: ActionType.SoftWarn });
            var warn = actions.FirstOrDefault(e => e.FacilityAction is { ActionType: ActionType.Warning });
            var alrm = actions.FirstOrDefault(e => e.FacilityAction is { ActionType: ActionType.Alarm });

            var t1 = CreateAnalogInput(device, "A1-Temperature", 1, soft, warn, alrm, "A1-Temperature", 0, 1, false, 30, 40, 50,tSensor);
            var h1 = CreateAnalogInput(device, "A1-Humidity", 1, soft, warn, alrm, "A1-Humidity",2, 1, false, 50, 60, 70,hSensor);
            
            var t2 = CreateAnalogInput(device, "A2-Temperature", 1, soft, warn, alrm, "A2-Temperature", 2, 1, false, 30, 40, 50,tSensor);
            var h2 = CreateAnalogInput(device, "A2-Humidity", 1, soft, warn, alrm, "A2-Humidity",3, 1, false, 50, 60, 70,hSensor);
            
            var t3 = CreateAnalogInput(device, "A3-Temperature", 1, soft, warn, alrm, "A3-Temperature", 4, 1, false, 30, 40, 50,tSensor);
            var h3 = CreateAnalogInput(device, "A3-Humidity", 1, soft, warn, alrm, "A3-Humidity",5, 1, false, 50, 60, 70,hSensor);
            
            var t4 = CreateAnalogInput(device, "A4-Temperature", 1, soft, warn, alrm, "A4-Temperature", 6, 1, false, 30, 40, 50,tSensor);
            var h4 = CreateAnalogInput(device, "A4-Humidity", 1, soft, warn, alrm, "A4-Humidity",7, 1, false, 50, 60, 70,hSensor);
            
            var t5 = CreateAnalogInput(device, "A5-Temperature", 1, soft, warn, alrm, "A5-Temperature", 8, 1, false, 30, 40, 50,tSensor);
            var h5 = CreateAnalogInput(device, "A5-Humidity", 1, soft, warn, alrm, "A5-Humidity",9, 1, false, 50, 60, 70,hSensor);
            
            var t6 = CreateAnalogInput(device, "A6-Temperature", 1, soft, warn, alrm, "A6-Temperature", 10, 1, false, 30, 40, 50,tSensor);
            var h6 = CreateAnalogInput(device, "A6-Humidity", 1, soft, warn, alrm, "A6-Humidity",11, 1, false, 50, 60, 70,hSensor);

            
            return new List<AnalogInput>() { t1,h1,t2,h2,t3,h3,t4,h4,t5,h5,t6,h6};
        }

        public static async Task CreateDeviceActions(string deviceName) {
            Console.WriteLine("Creating DeviceActions, Please Wait...");
            var context = new MonitorContext();
            var device = context.Devices.OfType<ModbusDevice>()
                .Include(e => e.Channels)
                .AsTracking()
                .FirstOrDefault(e => e.Name == deviceName);

            var facilityActions = context.FacilityActions
                .AsTracking()
                .ToList();
            var deviceActions = new List<DeviceAction>();
            foreach (var facilityAction in facilityActions) {
                if (facilityAction.ActionType != ActionType.Custom &&
                    facilityAction.ActionType != ActionType.Maintenance) {
                    deviceActions.Add(new DeviceAction() {
                        Id=Guid.NewGuid(),
                        Name=facilityAction.Name,
                        FacilityAction = facilityAction,
                        FacilityActionId = facilityAction.Id,
                        ModbusDevice = device,
                        ModbusDeviceId = device.Id,
                        FirmwareId = -1
                    });
                }
            }
            context.AddRange(deviceActions);
            await context.SaveChangesAsync();
            Console.WriteLine("DeviceActions Created, Checked Database");
        }

        public static AnalogInput CreateAnalogInput(ModbusDevice device,string id,int ch,
            DeviceAction soft,DeviceAction warn,DeviceAction alrm,
            string name, int reg,int regCount,bool enAlert,
            int setsoft,int setwarn,int setalrm,Sensor sensor) {
            AnalogInput input = new AnalogInput();
            input.Identifier = id;
            input.SystemChannel = ch;
            input.DisplayName = name;
            input.Connected = true;
            input.Bypass = false;
            input.Display = true;
            input.ModbusDevice = device;
            input.Sensor = sensor;
            input.SensorId = sensor.Id;

            ModbusAddress address = new ModbusAddress();
            address.RegisterType = ModbusRegister.Holding;
            address.Address = reg;
            address.RegisterLength = regCount;
            input.ModbusAddress = address;

            AnalogAlert alert = new AnalogAlert();
            alert.AlertLevels = new List<AnalogLevel>();
            alert.InputChannel = input;
            alert.Name = input.DisplayName;
            alert.Enabled = enAlert;
            alert.Bypass = false;
            alert.BypassResetTime = 24;

            var softwarn = new AnalogLevel();

                softwarn.DeviceAction = soft;
                softwarn.DeviceActionId = soft.Id;

            softwarn.SetPoint = setsoft;
            softwarn.Bypass = false;
            softwarn.Enabled = enAlert;
            softwarn.BypassResetTime = 24;

            var warning = new AnalogLevel();

            warning.DeviceAction = warn;
            warning.DeviceActionId = warn.Id;
            
            warning.SetPoint = setwarn;
            warning.Bypass = false;
            warning.Enabled = enAlert;
            warning.BypassResetTime = 24;

            var alarm = new AnalogLevel();

            alarm.DeviceAction = alrm;
            alarm.DeviceActionId = alarm.Id;
            
            alarm.SetPoint = setalrm;
            alarm.Bypass = false;
            alarm.Enabled = enAlert;
            alarm.BypassResetTime = 24;

            alert.AlertLevels.Add(softwarn);
            alert.AlertLevels.Add(warning);
            alert.AlertLevels.Add(alarm);
            input.Alert = alert;
            return input;
        }

        public static async Task CreateNHDevice() {
            Console.WriteLine("Creating Device,Please wait");
            using var context = new MonitorContext();
            var device = new ModbusDevice();
            device.Name = "nh3";
            device.HubAddress = @"http:\\nhstream\hubs\nhstreaming";
            device.HubName = "nhstreaming";

            var netConfig = new NetworkConfiguration();
            netConfig.IpAddress = "172.21.100.29";
            netConfig.Port = 502;
            netConfig.Dns = "172.20.3.5";
            netConfig.Mac = "";
            netConfig.Gateway = "172.21.100.1";

            var modbusConfig = new ModbusConfiguration();
            modbusConfig.SlaveAddress = 1;
            modbusConfig.Coils = 0;
            modbusConfig.HoldingRegisters = 70;
            modbusConfig.InputRegisters = 0;
            modbusConfig.DiscreteInputs = 0;

            ModbusChannelRegisterMap channelMapping = new ModbusChannelRegisterMap();
            channelMapping.AnalogRegisterType = ModbusRegister.Holding;
            channelMapping.AnalogStart = 0;
            channelMapping.AnalogStop = 69;
            device.ModbusConfiguration = modbusConfig;
            device.ChannelRegisterMap = channelMapping;
            device.NetworkConfiguration = netConfig;
            await context.AddAsync(device);
            await context.SaveChangesAsync();
            Console.WriteLine("Check Database");
        }
        
        public static async Task CreateTempMonitorDevice() {
            Console.WriteLine("Creating Device,Please wait");
            using var context = new MonitorContext();
            var device = new ModbusDevice();
            device.Name = "th";
            device.HubAddress = @"http:\\thmonitor\hubs\thstreaming";
            device.HubName = "thstreaming";

            var netConfig = new NetworkConfiguration();
            netConfig.IpAddress = "172.20.5.222";
            netConfig.Port = 502;
            netConfig.Dns = "172.20.3.5";
            netConfig.Mac = "";
            netConfig.Gateway = "172.20.5.1";

            var modbusConfig = new ModbusConfiguration();
            modbusConfig.SlaveAddress = 1;
            modbusConfig.Coils = 0;
            modbusConfig.HoldingRegisters = 12;
            modbusConfig.InputRegisters = 0;
            modbusConfig.DiscreteInputs = 0;

            ModbusChannelRegisterMap channelMapping = new ModbusChannelRegisterMap();
            channelMapping.AnalogRegisterType = ModbusRegister.Holding;
            channelMapping.AnalogStart = 0;
            channelMapping.AnalogStop = 11;
            device.ModbusConfiguration = modbusConfig;
            device.ChannelRegisterMap = channelMapping;
            device.NetworkConfiguration = netConfig;
            await context.AddAsync(device);
            await context.SaveChangesAsync();
            Console.WriteLine("Check Database");
        }

        public static async Task TestingMongoChanges() {
            Console.WriteLine("Testing MongoChange");
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("epi1_data_test");
            var collection = database.GetCollection<AnalogItem>("analog_items");
            var analogItems = await collection.Find(_ => true).ToListAsync();
            var update = Builders<AnalogItem>.Update
                .Set(e => e.Register, 0)
                .Set(e => e.RegisterLength, 2);

            foreach (var item in analogItems) {
                await collection.UpdateOneAsync(e => e._id == item._id, update);
            }

            analogItems = await collection.Find(_ => true).ToListAsync();
            foreach (var item in analogItems) {
                Console.WriteLine($"Item: {item.Identifier} Reg: {item.Register} RegLength: {item.RegisterLength}");
            }

            Console.WriteLine("Completed,Press Any Key To Exit");
            Console.ReadKey();
        }
        
        public static async Task DeleteDevice() {
            await using var context = new MonitorContext();
            var nhDevice = context.Devices.OfType<ModbusDevice>().AsTracking().FirstOrDefault(e => e.Name == "nh3");
            if (nhDevice != null) {
                Console.WriteLine("Removing Device..");
                context.Remove(nhDevice);
                await context.SaveChangesAsync();
                Console.WriteLine("Remove Done, Check Database..");
            } else {
                Console.WriteLine("Device not found");
            }
        }

        public static async Task DeleteChannels() {
            await using var context = new MonitorContext();
            var nhDevice = context.Devices.OfType<ModbusDevice>().FirstOrDefault(e => e.Name == "nh3");
            var channels = context.Channels.OfType<AnalogInput>()
                .Include(e=>e.Alert)
                .ThenInclude(e=>((AnalogAlert)e).AlertLevels)
                .Where(e => e.ModbusDeviceId == nhDevice.Id).ToList();
            Console.WriteLine("Removing Device..");
            context.RemoveRange(channels);
            await context.SaveChangesAsync();
            Console.WriteLine("Remove Done, Check Database..");
        }

    }
}
