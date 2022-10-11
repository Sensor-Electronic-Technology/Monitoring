﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MonitoringConfig.Data.Model;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringSystem.Shared.Data.LogModel;

namespace MonitoringSystem.ConsoleTesting {
    public class TestingNH {
        public static async Task Main(string[] args) {
            await CreateNH3Channels();
        }

        public static async Task CreateNH3Channels() {
            var context = new MonitorContext();
            var device = context.Devices.OfType<MonitorBox>()
                .Include(e => e.Channels)
                .AsTracking()
                .FirstOrDefault(e => e.Name == "nh3");

            var actions = context.FacilityActions
                .AsTracking()
                .ToList();

            var inputs=CreateChannels(device, actions);
            context.AddRange(inputs);
            await context.SaveChangesAsync();
            Console.WriteLine("Check Database");
        }

        public static IList<AnalogInput> CreateChannels(MonitorBox device,IList<FacilityAction> actions) {
            var soft = actions.FirstOrDefault(e => e.ActionType == ActionType.SoftWarn);
            var warn = actions.FirstOrDefault(e => e.ActionType == ActionType.Warning);
            var alrm = actions.FirstOrDefault(e => e.ActionType == ActionType.Alarm);

            var tank1 = CreateAnalogInput(device, "Tank1", 1, soft, warn, alrm, "Tank1 Weight", 0, 2, true, 200, 150, 100);
            var tank2 = CreateAnalogInput(device, "Tank1", 1, soft, warn, alrm, "Tank2 Weight", 0, 2, true, 200, 150, 100);
            var temp1 = CreateAnalogInput(device, "Temp1", 1, soft, warn, alrm, "Tank1 Temp.", 0, 1, false, 0, 0, 0);
            var temp2 = CreateAnalogInput(device, "Temp2", 1, soft, warn, alrm, "Tank2 Temp.", 0, 1, false, 0, 0, 0);
            var h1 = CreateAnalogInput(device, "Heater1", 1, soft, warn, alrm, "Heater1 Duty Cycle", 0, 1, false, 0, 0, 0);
            var h2 = CreateAnalogInput(device, "Heater2", 1, soft, warn, alrm, "Heater1 Duty Cycle", 0, 1, false, 0, 0, 0);
            return new List<AnalogInput>() { tank1, tank2, temp1, temp2, h1, h2 };
        }

        public static AnalogInput CreateAnalogInput(MonitorBox device,string id,int ch,
            FacilityAction soft,FacilityAction warn,FacilityAction alrm,
            string name, int reg,int regCount,bool enAlert,
            int setsoft,int setwarn,int setalrm) {
            AnalogInput input = new AnalogInput();
            input.Identifier = id;
            input.SystemChannel = ch;
            input.DisplayName = name;
            input.Connected = true;
            input.Bypass = false;
            input.Display = true;
            input.ModbusDevice = device;

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
            if (enAlert) {
                var deviceAction = new DeviceAction() {
                    Id = Guid.NewGuid(),
                    FacilityAction = soft,
                    FacilityActionId = soft.Id,
                    MonitorBox=device,
                    MonitorBoxId = device.Id,
                    FirmwareId = 0
                };
                softwarn.DeviceAction = deviceAction;
                softwarn.DeviceActionId = soft.Id;
            }
            softwarn.SetPoint = setsoft;
            softwarn.Bypass = false;
            softwarn.Enabled = enAlert;
            softwarn.BypassResetTime = 24;

            var warning = new AnalogLevel();
            if (enAlert) {
                var deviceAction = new DeviceAction() {
                    Id = Guid.NewGuid(),
                    FacilityAction = soft,
                    FacilityActionId = soft.Id,
                    MonitorBox=device,
                    MonitorBoxId = device.Id,
                    FirmwareId = 0
                };
                warning.DeviceAction = deviceAction;
                warning.DeviceActionId = warn.Id;
            }
            warning.SetPoint = setwarn;
            warning.Bypass = false;
            warning.Enabled = enAlert;
            warning.BypassResetTime = 24;

            var alarm = new AnalogLevel();
            if (enAlert) {
                var deviceAction = new DeviceAction() {
                    Id = Guid.NewGuid(),
                    FacilityAction = alrm,
                    FacilityActionId = alrm.Id,
                    MonitorBox=device,
                    MonitorBoxId = device.Id,
                    FirmwareId = 0
                };
                alarm.DeviceAction = deviceAction;
                alarm.DeviceActionId = alarm.Id;
            }
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
            Console.ReadKey();
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

    }
}
