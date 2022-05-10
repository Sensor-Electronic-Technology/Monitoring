using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringSystem.Shared.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.ConsoleTesting {
    public class FacilityParser {

        public static string boxId = "gasbay";
        public static string boxAnalogPath = $@"C:\MonitorFiles\{boxId}\ANALOG.TXT";
        public static string boxActionPath = $@"C:\MonitorFiles\{boxId}\ACTIONS.TXT";
        public static string boxDiscretePath = $@"C:\MonitorFiles\{boxId}\DIGITAL.TXT";
        public static string boxVirtualPath = $@"C:\MonitorFiles\{boxId}\VIRTUAL.TXT";
        public static string boxOutputPath = $@"C:\MonitorFiles\{boxId}\OUTPUT.TXT";
        public static string boxNetworkPath = $@"C:\MonitorFiles\{boxId}\NETWORK.TXT";

        public static void CreateAnalogInputs() {
            Console.WriteLine("Creating EpiLab1 AnalogInputs");
            var context = new FacilityContext();
            var monitoringBox = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .Include(e => e.Modules)
                .AsTracking()
                .FirstOrDefault(e => e.Identifier == boxId);

            var facilityActions = context.FacilityActions
                .Include(e => e.ActionOutputs)
                    .ThenInclude(e => e.Output)
                .AsTracking()
                .ToList();

            if (monitoringBox != null && facilityActions.Count > 0) {
                Console.WriteLine("Found Monitoring Box: {0}", monitoringBox.DisplayName);
                var dInputs = ParseAnalogInputs(facilityActions, monitoringBox);
                context.AddRange(dInputs);
                var ret = context.SaveChanges();
                if (ret > 0) {
                    Console.WriteLine("DiscreteInputs should be added");
                } else {
                    Console.WriteLine("Error adding DiscreteInputs");
                }
            } else {
                Console.WriteLine("Error: Could not find monitoring box");
            }
        }
        public static void CreateDiscreteInputs() {
            Console.WriteLine("Creating EpiLab2 DiscreteInputs");
            var context = new FacilityContext();
            var monitoringBox = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .Include(e => e.Modules)
                .AsTracking()
                .FirstOrDefault(e => e.Identifier == boxId);

            var actions = context.FacilityActions.AsTracking().ToList();

            if (monitoringBox != null) {
                Console.WriteLine("Found Monitoring Box: {0}", monitoringBox.DisplayName);
                var dInputs = ParseDiscreteInputs(monitoringBox, actions);
                context.AddRange(dInputs);
                var ret = context.SaveChanges();
                if (ret > 0) {
                    Console.WriteLine("DiscreteInputs should be added");
                } else {
                    Console.WriteLine("Error creating DiscreteInputs");
                }

            } else {
                Console.WriteLine("Error: Could not find monitoring box");
            }
        }
        public static void CreateVirtualInputs() {
            Console.WriteLine("Creating EpiLab2 VirtualInputs");
            var context = new FacilityContext();
            var monitoringBox = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .Include(e => e.Modules)
                .AsTracking()
                .FirstOrDefault(e => e.Identifier == boxId);

            var actions = context.FacilityActions.AsTracking().ToList();

            if (monitoringBox != null) {
                Console.WriteLine("Found Monitoring Box: {0}", monitoringBox.DisplayName);
                var dInputs = ParseVirtualChannels(monitoringBox, actions);
                context.AddRange(dInputs);
                var ret = context.SaveChanges();
                if (ret > 0) {
                    Console.WriteLine("VirtualInputs should be added");
                } else {
                    Console.WriteLine("Error creating VirtualInputs");
                }

            } else {
                Console.WriteLine("Error: Could not find monitoring box");
            }
        }
        public static void CreateOutputs() {
            Console.WriteLine("Creating EpiLab2 Outputs");
            var context = new FacilityContext();
            var monitoringBox = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .Include(e => e.Modules)
                .AsTracking()
                .FirstOrDefault(e => e.Identifier == boxId);

            if (monitoringBox != null) {
                Console.WriteLine("Found Monitoring Box: {0}", monitoringBox.DisplayName);
                var outputs = ParseOutputs(monitoringBox);
                context.AddRange(outputs);
                var ret = context.SaveChanges();
                if (ret > 0) {
                    Console.WriteLine("Outputs should be added");
                } else {
                    Console.WriteLine("Error adding channel outputs");
                }

            } else {
                Console.WriteLine("Error: Could not fing monitoring box");
            }
        }
        public static void CreateModbusDevices() {
            using var context = new FacilityContext();
            var modules = context.Modules.ToList();
            MonitoringBox box = new MonitoringBox();

            box.NetworkConfiguration = ParseNetworkConfiguration();
            box.Identifier = boxId;
            box.DisplayName = boxId;
            box.Status = "Normal";
            box.BypassAlarms = false;
            box.ReadInterval = 5;
            box.SaveInterval = 10;
            box.Modules = context.Modules.ToList();
            context.Devices.Add(box);
            var ret = context.SaveChanges();
            if (ret > 0) {
                Console.WriteLine("Monitoring Box added");
            } else {
                Console.WriteLine("Failed to add Monitoring Box");
            }
        }
        public static void CreateFacilityActions() {
            Console.WriteLine("Creating Monitoring Box EpiLab2");
            var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .AsTracking()
                .FirstOrDefault(e => e.Identifier == boxId);
            var actions = context.FacilityActions.AsTracking().ToList();
            //var outputs = context.Channels.OfType<DiscreteOutput>().AsTracking().ToList();
            if (monitoring != null) {
                var outputs = monitoring.Channels.OfType<DiscreteOutput>().OrderBy(e => e.SystemChannel).ToList();
                var actionMapping = ParseActionMapping(outputs, actions, monitoring);
                context.AddRange(actionMapping);
                var ret = context.SaveChanges();
                if (ret > 0) {
                    Console.WriteLine("FacilityActions should be created");
                } else {
                    Console.WriteLine("Error creating FacilityActions");
                }
            } else {
                Console.WriteLine("Error: Not outputs found");
            }
        }
        public static NetworkConfiguration ParseNetworkConfiguration() {
            var arr = JArray.Parse(File.ReadAllText(boxNetworkPath));
            NetworkConfiguration netConfig = new NetworkConfiguration();
            netConfig.ModbusConfig = new ModbusConfig();
            ChannelRegisterMapping channelMapping = new ChannelRegisterMapping();
            channelMapping.AnalogRegisterType = ModbusRegister.Input;
            channelMapping.AnalogStart = 0;
            channelMapping.AnalogStop = 7;

            channelMapping.DiscreteRegisterType = ModbusRegister.DiscreteInput;
            channelMapping.DiscreteStart = 0;
            channelMapping.DiscreteStop = 15;

            channelMapping.OutputRegisterType = ModbusRegister.DiscreteInput;
            channelMapping.OutputStart = 16;
            channelMapping.OutputStop = 23;

            channelMapping.ActionRegisterType = ModbusRegister.DiscreteInput;
            channelMapping.ActionStart = 24;
            channelMapping.ActionStop = 29;

            channelMapping.VirtualRegisterType = ModbusRegister.Coil;
            channelMapping.VirtualStart = 0;
            channelMapping.VirtualStop = 3;

            channelMapping.AlertRegisterType = ModbusRegister.Holding;
            channelMapping.AlertStart = 0;
            channelMapping.AlertStop = 27;

            channelMapping.DeviceRegisterType = ModbusRegister.Holding;
            channelMapping.DeviceStart = 28;
            channelMapping.DeviceStop = 28;

            netConfig.ModbusConfig.ChannelMapping = channelMapping;

            var config = arr[0];
            netConfig.IPAddress = config["IP"].Value<string>();
            netConfig.DNS = config["DNS"].Value<string>();
            netConfig.MAC = config["Mac"].Value<string>();
            netConfig.Gateway = config["Gateway"].Value<string>();

            ModbusAddress modbusAddress = new ModbusAddress() {
                Address = config["Register"].Value<int>(),
                RegisterType = (ModbusRegister)config["Type"].Value<int>(),
                RegisterLength = 1
            };
            netConfig.ModbusConfig.ModbusAddress = modbusAddress;
            netConfig.ModbusConfig.DiscreteInputs = config["DiscreteInputs"].Value<int>();
            netConfig.ModbusConfig.InputRegisters = config["InputRegsters"].Value<int>();
            netConfig.ModbusConfig.Coils = config["Coils"].Value<int>();
            netConfig.ModbusConfig.HoldingRegisters = config["HoldingRegister"].Value<int>();
            netConfig.Port = 502;
            netConfig.ModbusConfig.SlaveAddress = 1;
            return netConfig;
        }
        static IList<DiscreteInput> ParseDiscreteInputs(ModbusDevice modbusDevice, IList<FacilityAction> actions) {
            JArray channelArray = JArray.Parse(File.ReadAllText(boxDiscretePath));
            List<DiscreteInput> inputs = new List<DiscreteInput>();
            foreach (var elem in channelArray) {
                DiscreteInput input = new DiscreteInput();
                input.SystemChannel = elem["Input"].Value<int>();
                input.Identifier = "Discrete " + input.SystemChannel;
                input.DisplayName = input.Identifier;
                input.ChannelAddress = new ChannelAddress();
                input.ChannelAddress.Channel = elem["Address"]["Channel"].Value<int>();
                input.ChannelAddress.ModuleSlot = elem["Address"]["Slot"].Value<int>();

                input.ModbusAddress = new ModbusAddress();
                input.ModbusAddress.Address = elem["MRI"]["Register"].Value<int>();
                input.ModbusAddress.RegisterType = (ModbusRegister)elem["MRI"]["Type"].Value<int>();
                input.ModbusAddress.RegisterLength = 1;

                input.Connected = elem["Connected"].Value<bool>();
                input.ModbusDevice = modbusDevice;
                input.ModbusDeviceId = modbusDevice.Id;
                DiscreteAlert alert = new DiscreteAlert();

                alert.ModbusAddress = new ModbusAddress();
                alert.ModbusAddress.Address = elem["MRA"]["Register"].Value<int>();
                alert.ModbusAddress.RegisterType = (ModbusRegister)elem["MRA"]["Type"].Value<int>();
                alert.ModbusAddress.RegisterLength = 1;


                DiscreteLevel level = new DiscreteLevel();
                level.TriggerOn = elem["Alert"]["TriggerOn"].Value<int>() == 1 ? DiscreteState.High : DiscreteState.Low;
                int actionId = elem["Alert"]["Action"].Value<int>() + 1;
                if (actionId >= 1) {
                    level.FacilityActionId = actionId;
                }
                alert.Bypass = elem["Alert"]["Bypass"].Value<bool>();
                alert.Enabled = elem["Alert"]["Enabled"].Value<bool>();
                level.Bypass = alert.Bypass;
                level.BypassResetTime = 24;
                alert.BypassResetTime = level.BypassResetTime;
                input.Alert = alert;
                inputs.Add(input);
            }
            foreach (var input in inputs) {
                Console.WriteLine("Input: {0} AddrChannel: {1} AddrSlot: {2}", input.SystemChannel, input.ChannelAddress.Channel, input.ChannelAddress.ModuleSlot);
            }
            return inputs;
        }
        static IList<ModbusActionMap> ParseActionMapping(IList<DiscreteOutput> outputs, IList<FacilityAction> actions, MonitoringBox box) {
            var actionArr = JArray.Parse(File.ReadAllText(boxActionPath));
            IList<ModbusActionMap> actionMapping = actionArr.Select(p => CreateActionMapping(p, outputs, actions, box)).ToList();
            //foreach (var actionMap in actionMapping) {
            //    Console.WriteLine("ActionName: {0}, ActionType: {1}  ActionOutputs", actionMap.FacilityAction.ActionName, action.ActionType);
            //    foreach (var aoutput in action.ActionOutputs) {
            //        if (aoutput.Output is null) {
            //            Console.WriteLine("Output: {0} No Output", aoutput.OffLevel);
            //        } else {
            //            Console.WriteLine("Output: {0} AddrChannel: {1} AddrSlot: {2}",
            //                aoutput.Output.SystemChannel, aoutput.Output.ChannelAddress.Channel, aoutput.Output.ChannelAddress.ModuleSlot);
            //        }
            //    }
            //}
            return actionMapping;
        }

        public static ModbusActionMap CreateActionMapping(JToken p, IList<DiscreteOutput> outputs, IList<FacilityAction> actions, MonitoringBox box) {
            var actionId = p["ActionId"].Value<int>() + 1;
            var action = actions.FirstOrDefault(e => e.Id == actionId);
            if (action != null) {
                ModbusActionMap actionMap = new ModbusActionMap();
                ChannelAddress addr1 = new ChannelAddress() {
                    Channel = p["O1"]["Address"]["Channel"].Value<int>(),
                    ModuleSlot = p["O1"]["Address"]["Slot"].Value<int>()
                };

                ChannelAddress addr2 = new ChannelAddress() {
                    Channel = p["O2"]["Address"]["Channel"].Value<int>(),
                    ModuleSlot = p["O2"]["Address"]["Slot"].Value<int>()
                };

                ChannelAddress addr3 = new ChannelAddress() {
                    Channel = p["O3"]["Address"]["Channel"].Value<int>(),
                    ModuleSlot = p["O3"]["Address"]["Slot"].Value<int>()
                };

                var out1 = outputs.FirstOrDefault(e => e.ChannelAddress.Channel == addr1.Channel && e.ChannelAddress.ModuleSlot == addr1.ModuleSlot);
                var out2 = outputs.FirstOrDefault(e => e.ChannelAddress.Channel == addr2.Channel && e.ChannelAddress.ModuleSlot == addr2.ModuleSlot);
                var out3 = outputs.FirstOrDefault(e => e.ChannelAddress.Channel == addr3.Channel && e.ChannelAddress.ModuleSlot == addr3.ModuleSlot);

                ActionOutput aout1 = new ActionOutput() {
                    Output = out1,
                    OnLevel = p["O1"]["OnLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
                    OffLevel = p["O1"]["OffLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
                };
                ActionOutput aout2 = new ActionOutput() {
                    Output = out2,
                    OnLevel = p["O2"]["OnLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
                    OffLevel = p["O2"]["OffLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
                };
                ActionOutput aout3 = new ActionOutput() {
                    Output = out3,
                    OnLevel = p["O3"]["OnLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
                    OffLevel = p["O3"]["OffLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
                };

                action.ActionOutputs.Add(aout1);
                action.ActionOutputs.Add(aout2);
                action.ActionOutputs.Add(aout3);

                actionMap.FacilityAction = action;
                actionMap.FacilityActionId = action.Id;
                actionMap.MonitoringBox = box;
                actionMap.MonitoringBoxId = box.Id;
                actionMap.ModbusAddress = new ModbusAddress();
                actionMap.ModbusAddress.Address = p["Register"].Value<int>();
                actionMap.ModbusAddress.RegisterType = (ModbusRegister)p["Type"].Value<int>();
                actionMap.ModbusAddress.RegisterLength = 1;
                return actionMap;
            } else {
                return null;
            }
        }

        //public static FacilityAction CreateFacilityAction(JToken p, IList<DiscreteOutput> outputs,MonitoringBox box) {
        //    FacilityAction action = new FacilityAction();
        //    action.Id = p["ActionId"].Value<int>() + 1;
        //    action.ActionName = ToActionType(p["ActionType"].Value<int>()).ToString();
        //    action.ActionOutputs = new List<ActionOutput>();

        //    ChannelAddress addr1 = new ChannelAddress() {
        //        Channel = p["O1"]["Address"]["Channel"].Value<int>(),
        //        ModuleSlot = p["O1"]["Address"]["Slot"].Value<int>()
        //    };
        //    ChannelAddress addr2 = new ChannelAddress() {
        //        Channel = p["O2"]["Address"]["Channel"].Value<int>(),
        //        ModuleSlot = p["O2"]["Address"]["Slot"].Value<int>()
        //    };
        //    ChannelAddress addr3 = new ChannelAddress() {
        //        Channel = p["O3"]["Address"]["Channel"].Value<int>(),
        //        ModuleSlot = p["O3"]["Address"]["Slot"].Value<int>()
        //    };

        //    var out1 = outputs.FirstOrDefault(e => e.ChannelAddress.Channel == addr1.Channel && e.ChannelAddress.ModuleSlot == addr1.ModuleSlot);
        //    var out2 = outputs.FirstOrDefault(e => e.ChannelAddress.Channel == addr2.Channel && e.ChannelAddress.ModuleSlot == addr2.ModuleSlot);
        //    var out3 = outputs.FirstOrDefault(e => e.ChannelAddress.Channel == addr3.Channel && e.ChannelAddress.ModuleSlot == addr3.ModuleSlot);

        //    ActionOutput aout1 = new ActionOutput() {
        //        Output = out1,
        //        OnLevel = p["O1"]["OnLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
        //        OffLevel = p["O1"]["OffLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
        //    };
        //    ActionOutput aout2 = new ActionOutput() {
        //        Output = out2,
        //        OnLevel = p["O2"]["OnLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
        //        OffLevel = p["O2"]["OffLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
        //    };
        //    ActionOutput aout3 = new ActionOutput() {
        //        Output = out3,
        //        OnLevel = p["O3"]["OnLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
        //        OffLevel = p["O3"]["OffLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
        //    };
        //    action.ActionOutputs.Add(aout1);
        //    action.ActionOutputs.Add(aout2);
        //    action.ActionOutputs.Add(aout3);
        //    action.ActionType = ToActionType(p["ActionType"].Value<int>());
        //    ModbusActionMap actionMap = new ModbusActionMap();
        //    actionMap.FacilityAction = action;
        //    actionMap.MonitoringBox = box;
        //    actionMap.ModbusAddress = new ModbusAddress();
        //    actionMap.ModbusAddress.Address = p["Register"].Value<int>();
        //    actionMap.ModbusAddress.RegisterType = (ModbusRegister)p["Type"].Value<int>();
        //    actionMap.ModbusAddress.RegisterLength = 1;
        //    //action.ModbusActionMap = actionMap;

        //    return action;
        //}
        static IList<AnalogInput> ParseAnalogInputs(IList<FacilityAction> actions, ModbusDevice modDevice) {
            using var context = new FacilityContext();
            var sensor = context.Sensors.FirstOrDefault(e => e.Name == "H2 Detector-PPM");
            if (sensor != null) {
                Console.WriteLine("Sensor Found!");
                Console.WriteLine("Name: {0} Slope: {1} Offset: {2}", sensor.Name, sensor.Slope, sensor.Offset);
            }
            var aInputs = JArray.Parse(File.ReadAllText(boxAnalogPath));
            IList<AnalogInput> analogInputs = aInputs.Select(p => CreateAnalogInput(p, actions, modDevice, sensor)).ToList();
            Console.WriteLine("AnalogInputs: ");
            int count = 0;
            foreach (var aInput in analogInputs) {
                if (aInput.Sensor != null) {
                    Console.WriteLine("Input: {0} Addr: {1} Slot: {2} Sensor: {3} Slope: {4} ", count, aInput.ChannelAddress.Channel, aInput.ChannelAddress.ModuleSlot, aInput.Sensor.Name, aInput.Sensor.Slope);
                } else {
                    Console.WriteLine("Input: {0} Addr: {1} Slot: {2} Sensor: No Sensor ", count, aInput.ChannelAddress.Channel, aInput.ChannelAddress.ModuleSlot);
                }

                count++;
                var alert = aInput.Alert as AnalogAlert;
                foreach (var level in alert.AlertLevels) {
                    if (level.FacilityAction != null) {
                        Console.WriteLine(" ActionId: {0} ActionType {1}", level.FacilityActionId, level.FacilityAction.ActionType);
                    } else {
                        Console.WriteLine("Alert Bypassed: {0}", alert.Bypass);
                    }
                }
            }
            return analogInputs;
        }
        static AnalogInput CreateAnalogInput(JToken token, IList<FacilityAction> actions, ModbusDevice modDevice, Sensor sensor) {
            AnalogInput aInput = new AnalogInput();
            aInput.SystemChannel = token["Input"].Value<int>();
            aInput.Identifier = "Analog " + aInput.SystemChannel;
            aInput.DisplayName = aInput.Identifier;
            aInput.ChannelAddress = new ChannelAddress();
            aInput.ChannelAddress.Channel = token["Address"]["Channel"].Value<int>();
            aInput.ChannelAddress.ModuleSlot = token["Address"]["Slot"].Value<int>();

            aInput.ModbusAddress = new ModbusAddress();
            aInput.ModbusAddress.Address = token["MRI"]["Register"].Value<int>();
            aInput.ModbusAddress.RegisterType = (ModbusRegister)token["MRI"]["Type"].Value<int>();
            aInput.ModbusAddress.RegisterLength = 1;

            aInput.Connected = token["Connected"].Value<bool>();
            aInput.ModbusDevice = modDevice;
            AnalogAlert alert = new AnalogAlert();
            alert.ModbusAddress = new ModbusAddress();
            alert.ModbusAddress.Address = token["MRA"]["Register"].Value<int>();
            alert.ModbusAddress.RegisterType = (ModbusRegister)token["MRA"]["Type"].Value<int>();
            alert.ModbusAddress.RegisterLength = 1;
            alert.AlertLevels = new List<AnalogLevel>();

            int a1 = token["A1"]["Action"].Value<int>() + 1;
            int a2 = token["A2"]["Action"].Value<int>() + 1;
            int a3 = token["A3"]["Action"].Value<int>() + 1;

            FacilityAction action1 = actions.FirstOrDefault(e => e.Id == a1);
            FacilityAction action2 = actions.FirstOrDefault(e => e.Id == a2);
            FacilityAction action3 = actions.FirstOrDefault(e => e.Id == a3);

            AnalogLevel level1 = new AnalogLevel() {
                SetPoint = token["A1"]["Setpoint"].Value<int>(),
                Bypass = token["A1"]["Bypass"].Value<bool>(),
                Enabled = token["A1"]["Enabled"].Value<bool>(),
                BypassResetTime = 24
            };
            if (action1 != null) {
                level1.FacilityActionId = action1.Id;
                level1.FacilityAction = action1;
            }

            AnalogLevel level2 = new AnalogLevel() {
                SetPoint = token["A2"]["Setpoint"].Value<int>(),
                Bypass = token["A2"]["Bypass"].Value<bool>(),
                Enabled = token["A2"]["Enabled"].Value<bool>(),
                BypassResetTime = 24
            };
            if (action2 != null) {
                level2.FacilityActionId = action2.Id;
                level2.FacilityAction = action2;
            }

            AnalogLevel level3 = new AnalogLevel() {
                SetPoint = token["A3"]["Setpoint"].Value<int>(),
                Bypass = token["A3"]["Bypass"].Value<bool>(),
                Enabled = token["A3"]["Enabled"].Value<bool>(),
                BypassResetTime = 24
            };

            if (action3 != null) {
                level3.FacilityActionId = action3.Id;
                level3.FacilityAction = action3;
            }

            alert.AlertLevels.Add(level1);
            alert.AlertLevels.Add(level2);
            alert.AlertLevels.Add(level3);
            alert.Bypass = false;
            alert.BypassResetTime = 24;
            aInput.Alert = alert;

            if (sensor != null) {
                if (aInput.Connected) {
                    aInput.SensorId = sensor.Id;
                }
            }
            return aInput;
        }
        public static IList<DiscreteOutput> ParseOutputs(ModbusDevice modbusDevice) {
            var outArr = JArray.Parse(File.ReadAllText(boxOutputPath));
            IList<DiscreteOutput> outputs = outArr.Select(p => new DiscreteOutput {
                ModbusDevice = modbusDevice,
                ModbusDeviceId = modbusDevice.Id,
                ChannelAddress = new ChannelAddress() {
                    Channel = p["Addr"]["Channel"].Value<int>(),
                    ModuleSlot = p["Addr"]["Module Slot"].Value<int>()
                },
                ModbusAddress = new ModbusAddress() {
                    Address = p["MRI"]["Register"].Value<int>(),
                    RegisterType = (ModbusRegister)p["MRI"]["Type"].Value<int>(),

                    RegisterLength = 1
                },
                SystemChannel = p["Output"].Value<int>(),
                Connected = p["Connected"].Value<bool>(),
                Identifier = "Output " + p["Output"].Value<int>().ToString(),
                DisplayName = "Output " + p["Output"].Value<int>().ToString(),
                StartState = p["Start State"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High
                //ChannelState = p["Start State"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High
            }).ToList();
            Console.WriteLine("DiscreteOutputs: ");
            foreach (var output in outputs) {
                Console.WriteLine("Input: {0} AddrChannel: {1} AddrSlot: {2}", output.SystemChannel, output.ChannelAddress.Channel, output.ChannelAddress.ModuleSlot);
            }
            return outputs;
        }
        public static IList<VirtualInput> ParseVirtualChannels(ModbusDevice modbusDevice, IList<FacilityAction> actions) {
            var vInputs = JArray.Parse(File.ReadAllText(boxVirtualPath));
            IList<VirtualInput> virtualInputs = vInputs.Select(e => CreateVirtualChannel(e, modbusDevice, actions)).ToList();
            foreach (var input in virtualInputs) {
                Console.WriteLine("VirtualInput: {0} ActionId: {1}", input.SystemChannel, (input.Alert as DiscreteAlert).AlertLevel.FacilityActionId);
            }
            return virtualInputs;
        }
        public static VirtualInput CreateVirtualChannel(JToken token, ModbusDevice modbusDevice, IList<FacilityAction> actions) {
            VirtualInput vInput = new VirtualInput();
            vInput.SystemChannel = token["Input"].Value<int>();
            vInput.Identifier = "Virtual " + vInput.SystemChannel;
            vInput.ChannelAddress = new ChannelAddress();
            vInput.ChannelAddress.Channel = 0;
            vInput.ChannelAddress.ModuleSlot = 0;

            vInput.ModbusAddress = new ModbusAddress();

            vInput.ModbusAddress.Address = token["MRI"]["Register"].Value<int>();
            vInput.ModbusAddress.RegisterType = (ModbusRegister)token["MRI"]["Type"].Value<int>();
            vInput.ModbusAddress.RegisterLength = 1;

            vInput.Connected = token["Connected"].Value<bool>();
            vInput.ModbusDevice = modbusDevice;
            vInput.ModbusDeviceId = modbusDevice.Id;
            DiscreteAlert alert = new DiscreteAlert();
            alert.ModbusAddress = new ModbusAddress() {
                Address = token["MRA"]["Register"].Value<int>(),
                RegisterType = (ModbusRegister)token["MRA"]["Type"].Value<int>(),
                RegisterLength = 1
            };

            DiscreteLevel level = new DiscreteLevel();

            level.TriggerOn = token["Alert"]["TriggerOn"].Value<int>() == 1 ? DiscreteState.High : DiscreteState.Low;
            int actionId = token["Alert"]["Action"].Value<int>() + 1;
            if (actionId >= 1) {
                level.FacilityActionId = actionId;
            }
            level.Bypass = token["Alert"]["Bypass"].Value<bool>();
            level.Enabled = token["Alert"]["Enabled"].Value<bool>();
            alert.Bypass = level.Bypass;
            alert.Enabled = level.Enabled;
            level.BypassResetTime = 24;
            alert.BypassResetTime = level.BypassResetTime;
            alert.AlertLevel = level;
            vInput.Alert = alert;
            return vInput;
        }
        public static ActionType ToActionType(int i) {
            switch (i) {
                case 1: {
                        return ActionType.Custom;
                    }
                case 2: {
                        return ActionType.Maintenance;
                    }
                case 3: {
                        return ActionType.SoftWarn;
                    }
                case 4: {
                        return ActionType.Warning;
                    }
                case 5: {
                        return ActionType.Alarm;
                    }
                case 6: {
                        return ActionType.Okay;
                    }
                default: {
                        return ActionType.Okay;
                    }
            }
        }
    }
}
