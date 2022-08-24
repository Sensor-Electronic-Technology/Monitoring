using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MailKit.Net.Smtp;
using MimeKit;
using MongoDB.Bson;
using MonitoringConfig.Data.Model;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Services;
using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringSystem.ConsoleTesting {
    public class Program {

        static async Task Main(string[] args) {
            //await CreateMongoDB("Epi1");
            //await BuildSettingsDB();
            await BuildEmailSettingsCollection();
        }
        static async Task RemoteAlertTesting() {
            ModbusService modservice = new ModbusService();
            Console.WriteLine("Epi1 Running");
            await modservice.WriteCoil("172.20.5.39", 502, 1, 2, true);
            await modservice.WriteCoil("172.20.5.39", 502, 1, 0, true);
            await Task.Delay(1000);
            await modservice.WriteCoil("172.20.5.39", 502, 1, 0, false);
            await modservice.WriteCoil("172.20.5.39", 502, 1, 2, false);
            
            Console.WriteLine("Gasbay running");
            await modservice.WriteCoil("172.20.5.42", 502, 1, 2, true);
            await modservice.WriteCoil("172.20.5.42", 502, 1, 0, true);
            await Task.Delay(1000);
            await modservice.WriteCoil("172.20.5.42", 502, 1, 0, false);
            await modservice.WriteCoil("172.20.5.42", 502, 1, 2, false);

            Console.WriteLine("Epi2 running");
            await modservice.WriteCoil("172.20.5.201", 502, 1, 2, true);
            await modservice.WriteCoil("172.20.5.201", 502, 1, 0, true);
            await Task.Delay(1000);
            await modservice.WriteCoil("172.20.5.201", 502, 1, 0, false);
            await modservice.WriteCoil("172.20.5.201", 502, 1, 2, false);
            Console.WriteLine("Test Done");
            
            /*bool state = false;
            for (int i = 0; i < 20; i++) {
                state = !state;
                await modservice.WriteCoil("172.20.5.42", 502, 1, 0, state);
                Console.WriteLine($"{DateTime.Now.ToString()}: {state}");
                await Task.Delay(1000);
            }
            await modservice.WriteCoil("172.20.5.42", 502, 1, 0, false);
            await modservice.WriteCoil("172.20.5.42", 502, 1, 2, false);*/
            
            //Console.WriteLine("Check System");
        }
        static async Task TestSmptEmail() {
            SmtpClient client = new SmtpClient();
            /*await client.ConnectAsync("smtp.gmail.com", 25, false);
            //await client.AuthenticateAsync("seti.monitoring@gmail.com", "Today@seti!");*/
            client.CheckCertificateRevocation = false;
            client.ServerCertificateValidationCallback = MyServerCertificateValidationCallback;
            await client.ConnectAsync("192.168.0.123",25,false);
            
            //await client.AuthenticateAsync("600076@seoulsemicon.com", "Drizzle3219753!");
            MimeMessage mailMessage = new MimeMessage();
            BodyBuilder bodyBuilder = new BodyBuilder();
            mailMessage.To.Add(new MailboxAddress("Andrew Elmendorf","aelmendorf@s-et.com"));
            mailMessage.From.Add(new MailboxAddress("Monitor Alerts","monitoring@s-et.com"));

            MessageBuilder builder = new MessageBuilder();
            builder.StartMessage("A Test");
            builder.AppendAlert("Alert 2","Alarm","50");
            builder.AppendStatus("Alert 1","Okay","0");
            builder.AppendStatus("Alert 2","Alarm","55");
            
            bodyBuilder.HtmlBody=builder.FinishMessage();
            mailMessage.Body = bodyBuilder.ToMessageBody();
            await client.SendAsync(mailMessage);
            await client.DisconnectAsync(true);
            Console.WriteLine("Check Mail");
        }
        
        static bool MyServerCertificateValidationCallback (object sender, 
            X509Certificate certificate, 
            X509Chain chain, 
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Note: The following code casts to an X509Certificate2 because it's easier to get the
            // values for comparison, but it's possible to get them from an X509Certificate as well.
            if (certificate is X509Certificate2 certificate2) {
                var cn = certificate2.GetNameInfo (X509NameType.SimpleName, false);
                var fingerprint = certificate2.Thumbprint;
                var serial = certificate2.SerialNumber;
                var issuer = certificate2.Issuer;
                Console.WriteLine($"Cert: {cn}");
                Console.WriteLine($"Fingerprint: {fingerprint}");
                Console.WriteLine($"Serial: {serial}");
                Console.WriteLine($"Issuer: {issuer}");
                return cn == "Exchange2016" && issuer == "CN=Exchange2016" &&
                       serial == "3D2E6FBDF9CE1FAF46D9CC68B8D58BAB" &&
                       fingerprint == "EC14ED8D2253824E6522D19EC815AD72CC767759";
                //return true;
            }
            return false;
        }
        
        static async Task BuildSettingsDB() { 
            var context = new MonitorContext();
            var devices = context.Devices.OfType<ModbusDevice>()
                .Include(e=>e.NetworkConfiguration)
                .Include(e=>e.ModbusConfiguration)
                .Include(e=>e.ChannelRegisterMap)
                .ToList();
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("monitor_settings_dev");
            var monitorDevCollection = database.GetCollection<ManagedDevice>("monitor_devices");
            var sensorCollection = database.GetCollection<SensorType>("sensor_types");
            var sensors=await context.Sensors.Select(e => new SensorType() {
                Name=e.Name,
                EntityId = e.Id.ToString(),
                Offset=e.Offset,
                Slope=e.Slope,
                Units=e.Units,
                YAxisStart=e.YAxisMin,
                YAxisStop=e.YAxisMin
            }).ToListAsync();
            foreach(var sensor in sensors) {
                sensor._id=ObjectId.GenerateNewId();
            }
            
            List<ManagedDevice> monitorDevices = new List<ManagedDevice>();
            foreach (var device in devices) {
                var channels = await context.Channels
                    .Include(e => ((AnalogInput)e).Sensor)
                    .Where(e=>e.ModbusDeviceId==device.Id)
                    .ToListAsync();
                var remoteActions = channels.OfType<VirtualInput>().Select(e => 
                    new RemoteAction() {
                        Name = e.DisplayName,
                        Register = e.ModbusAddress.Address,
                        State = false
                    }
                ).ToList();
                var entitySensors = channels.OfType<AnalogInput>().Where(e => e.SensorId != null)
                    .Select(e => e.SensorId.Value).Distinct();
                Console.WriteLine($"Sensor Count: {entitySensors.Count()}");
                ManagedDevice dev = new ManagedDevice();
                dev.DeviceId = device.Id.ToString();
                dev.DatabaseName = device.Name.ToLower()+"_data_dev";
                dev.DeviceName = device.Name;
                dev.DeviceType = device.GetType().Name;
                dev.ChannelMapping = new ChannelMappingConfigDto() {
                    AlertRegisterType = device.ChannelRegisterMap.AlertRegisterType,
                    AnalogRegisterType=device.ChannelRegisterMap.AnalogRegisterType,
                    DiscreteRegisterType = device.ChannelRegisterMap.DiscreteRegisterType,
                    VirtualRegisterType=device.ChannelRegisterMap.DiscreteRegisterType,
                    AlertStart=device.ChannelRegisterMap.AlertStart,
                    AlertStop=device.ChannelRegisterMap.AlertStart,
                    AnalogStart=device.ChannelRegisterMap.AnalogStart,
                    AnalogStop=device.ChannelRegisterMap.AnalogStop,
                    DiscreteStart=device.ChannelRegisterMap.DiscreteStart,
                    DiscreteStop=device.ChannelRegisterMap.DiscreteStop,
                    VirtualStart=device.ChannelRegisterMap.VirtualStart,
                    VirtualStop=device.ChannelRegisterMap.VirtualStop,
                };
                dev.ModbusConfiguration = new ModbusConfigDto() {
                    HoldingRegisters = device.ModbusConfiguration.HoldingRegisters,
                    DiscreteInputs = device.ModbusConfiguration.DiscreteInputs,
                    Coils=device.ModbusConfiguration.Coils,
                    InputRegisters = device.ModbusConfiguration.InputRegisters,
                    SlaveAddress=device.ModbusConfiguration.SlaveAddress
                };
                dev.IpAddress = device.NetworkConfiguration.IpAddress;
                dev.Port = device.NetworkConfiguration.Port;
                dev.HubAddress = device.HubAddress;
                dev.HubName = device.HubName;
                foreach (var sensorid in entitySensors) {
                    var found=sensors.FirstOrDefault(e => e.EntityId.ToLower() == sensorid.ToString());
                    Console.WriteLine($"SensorId: {sensorid.ToString()}");
                    if (found is not null) {
                        Console.WriteLine($"Found: {found.EntityId}");
                        dev.SensorTypes.Add(found._id);
                    }
                }
                dev.RemoteActions = remoteActions;
                dev.CollectionNames = new Dictionary<string, string>() {
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
                };
                monitorDevices.Add(dev);
            }
            await monitorDevCollection.InsertManyAsync(monitorDevices);
            await sensorCollection.InsertManyAsync(sensors);
            Console.WriteLine("Check Database");
        }
        public static async Task ToggleRemote() {
            ModbusService modservice = new ModbusService();
            /*var netConfig = gasbay.NetworkConfiguration;
            var modbusConfig = netConfig.ModbusConfig;*/
            Console.WriteLine("Starting test");
            await modservice.WriteCoil("172.20.5.39", 502, 1, 2, true);
            await modservice.WriteCoil("172.20.5.39", 502, 1, 1, true);
            await Task.Delay(5000);
            await modservice.WriteCoil("172.20.5.201", 502, 1, 2, true);
            await modservice.WriteCoil("172.20.5.201", 502, 1, 1, true);
            await Task.Delay(10000);
            await modservice.WriteCoil("172.20.5.39", 502, 1, 1, false);
            await modservice.WriteCoil("172.20.5.39", 502, 1, 2, false);
            await Task.Delay(5000);
            await modservice.WriteCoil("172.20.5.201", 502, 1, 1, false);
            await modservice.WriteCoil("172.20.5.201", 502, 1, 2, false);
            //await AlertItemTypeUpdate("gasbay");
            Console.WriteLine("Done");
        }
        /*static async Task BuildSensorCollection() {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("monitor_settings");
            var sensorCol = database.GetCollection<SensorType>("sensor_types");
            using var context = new MonitorContext();
            var sensors = await context.Sensors.Select(e=>new SensorType() {
                _id = ObjectId.Parse(e.Id.ToString()),
                Name = e.Name,
                Units=e.Units,
                YAxisStart = e.YAxisMin,
                YAxisStop = e.YAxitMax
            }).ToListAsync();
            await sensorCol.InsertManyAsync(sensors);
            Console.WriteLine("Done,Check Database");
        }*/
        static async Task BuildEmailSettingsCollection() {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("monitor_settings_dev");
            //await database.CreateCollectionAsync("monitor_devices");
            var emailCollection = database.GetCollection<EmailRecipient>("email_recipients");
            List<EmailRecipient> recipients = new List<EmailRecipient>();
            recipients.Add(new EmailRecipient() {
                Username = "Rakesh Jain",
                Address = "rakesh@s-et.com"
            });
            recipients.Add(new EmailRecipient() {
                Username = "Andrew Elmendorf",
                Address = "aelmendorf@s-et.com"
            });
            recipients.Add(new EmailRecipient() {
                Username = "Brad Murdaugh",
                Address = "bmurdaugh@s-et.com"
            });
            recipients.Add(new EmailRecipient() {
                Username = "Andy Chapman",
                Address = "achapman@s-et.com"
            });
            recipients.Add(new EmailRecipient() {
                Username = "Joe Dion",
                Address = "jdion@s-et.com"
            });

            recipients.Add(new EmailRecipient() {
                Username = "Lachab",
                Address="mlachab@s-et.com"
            });
            await emailCollection.InsertManyAsync(recipients);
            Console.WriteLine("Check Database");
            Console.ReadKey();
        }
       public static async Task CreateMongoDB(string deviceName) {
            using var context = new MonitorContext();
            var client = new MongoClient("mongodb://172.20.3.41");
            var device = context.Devices.OfType<MonitorBox>()
                .AsNoTracking()
                .FirstOrDefault(e => e.Name == deviceName);

            if(device is not null) {
                Console.WriteLine($"Device {device.Name} found");
                var database = client.GetDatabase($"{device.Name.ToLower()}_data_dev");
                Console.WriteLine("Creating Collections");
                Console.WriteLine($"Device {device.Name.ToLower()} found");

                Console.WriteLine("Creating Collections");
                await database.CreateCollectionAsync("analog_items");
                await database.CreateCollectionAsync("discrete_items");
                await database.CreateCollectionAsync("output_items");
                await database.CreateCollectionAsync("virtual_items");
                await database.CreateCollectionAsync("action_items");
                await database.CreateCollectionAsync("alert_items");

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

                IMongoCollection<AnalogItem> analogItems = database.GetCollection<AnalogItem>("analog_items");
                var analogChannels = await context.Channels.OfType<AnalogInput>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Include(e => e.Alert)
                    .Where(e => e.ModbusDevice.Name == device.Name)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new AnalogItem() {
                        Identifier = e.DisplayName,
                        SystemChannel = e.SystemChannel,
                        ItemId = e.Id.ToString(),
                        Factor = 10,
                        Display = e.Display,
                        Level1Action= ActionType.SoftWarn,
                        Level2Action=ActionType.Warning,
                        Level3Action=ActionType.Alarm,
                        Register=e.ModbusAddress.Address,
                        RegisterLength= e.ModbusAddress.RegisterLength
                    }).ToListAsync();
                
                /*var discreteItems = database.GetCollection<DiscreteChannel>("discrete_items");
                var discreteChannels = await context.Channels.OfType<DiscreteInput>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Name == device.Identifier)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new DiscreteChannel() {
                        identifier = e.DisplayName,
                        _id = e.Id,
                        display = e.Display
                    }).ToListAsync();

                IMongoCollection<OutputItem> outputItems = database.GetCollection<OutputItem>("output_items");
                var outputs = await context.Channels.OfType<OutputChannel>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new OutputItem() {
                        identifier = e.DisplayName,
                        _id = e.Id,
                        display = e.Display
                    }).ToListAsync();

                IMongoCollection<ActionItem> actionItems = database.GetCollection<ActionItem>("action_items");
                var actions = await context.FacilityActions
                    .AsNoTracking()
                    .Select(e => new ActionItem() {
                        identifier = e.ActionName,
                        _id = e.Id,
                        EmailEnabled = e.EmailEnabled,
                        EmailPeriod = e.EmailPeriod,
                        actionType = e.ActionType,
                        display = true
                    }).ToListAsync();

                IMongoCollection<VirtualChannel> virtualItems = database.GetCollection<VirtualChannel>("virtual_items");
                var virtualChannels = await context.Channels.OfType<VirtualInput>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new VirtualChannel() {
                        identifier = e.DisplayName,
                        _id = e.Id,
                        display = e.Display
                    }).ToListAsync();

                IMongoCollection<MonitorAlert> monitorAlerts = database.GetCollection<MonitorAlert>("alert_items");
                var alerts = await context.Channels.OfType<InputChannel>()
                    .AsNoTracking()
                    .Include(e => e.Alert)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .Select(e => e.Alert)
                    .Select(e => new MonitorAlert() {
                        _id = e.Id,
                        channelId = e.InputChannel.Id,
                        enabled = e.Enabled,
                        bypassed = e.Bypass,
                        displayName = e.DisplayName,
                        bypassResetTime = e.BypassResetTime,
                        itemType = e.AlertItemType,
                        latched = false,
                        CurrentState = ActionType.Okay
                    }).ToListAsync();*/

                //await deviceItems.InsertOneAsync(deviceConfig);
                await analogItems.InsertManyAsync(analogChannels);
                /*await discreteItems.InsertManyAsync(discreteChannels);
                await outputItems.InsertManyAsync(outputs);
                await actionItems.InsertManyAsync(actions);
                await monitorAlerts.InsertManyAsync(alerts);
                await virtualItems.InsertManyAsync(virtualChannels);*/
                Console.WriteLine("Check database, press any key to exit");
            } else {
                Console.WriteLine("Error: Device not found");
            }
            Console.ReadKey();
        }
       /*static async Task WriteOutAnalogFile(string deviceName, DateTime start, DateTime stop, string fileName) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceName + "_data");

            var analogItems = database.GetCollection<AnalogChannel>("analog_items").Find(_ => true).ToList();
            var analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
            Console.WriteLine("Starting query");
            var aReadings = await (await analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop)).ToListAsync();
            //var aReadings = await (await analogReadings.FindAsync(_=>true)).ToListAsync();
            var headers = analogItems.Select(e => e.identifier).ToList();
            StringBuilder hbuilder = new StringBuilder();
            hbuilder.Append("timestamp,");
            headers.ForEach((id) => {
                hbuilder.Append($"{id},");
            });
            Console.WriteLine($"Query Completed.  Count: {aReadings.Count()}");
            List<string> lines = new List<string>();
            lines.Add(hbuilder.ToString());
            foreach(var readings in aReadings) {
                StringBuilder builder = new StringBuilder();
                builder.Append(readings.timestamp.ToLocalTime().ToString()+",");
                foreach(var reading in readings.readings) {
                    builder.Append($"{reading.value},");
                }
                lines.Add(builder.ToString());
            }
            Console.WriteLine("Writing Out Data");
            File.WriteAllLines(fileName, lines);
            Console.WriteLine("Check File");
        }
       static async Task WriteOutDiscreteData(string deviceName, DateTime start, DateTime stop, string fileName) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceName + "_data");

            var discreteItems = database.GetCollection<DiscreteChannel>("discrete_items").Find(_ => true).ToList();
            var discreteReadings = database.GetCollection<DiscreteReadings>("discrete_readings");
            Console.WriteLine("Starting query");
            var dReadings = await (await discreteReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop)).ToListAsync();
            //var aReadings = await (await analogReadings.FindAsync(_=>true)).ToListAsync();
            var headers = discreteItems.Select(e => e.identifier).ToList();
            StringBuilder hbuilder = new StringBuilder();
            hbuilder.Append("timestamp,");
            headers.ForEach((id) => {
                hbuilder.Append($"{id},");
            });
            Console.WriteLine($"Query Completed.  Count: {dReadings.Count()}");
            List<string> lines = new List<string>();
            lines.Add(hbuilder.ToString());
            foreach(var readings in dReadings) {
                StringBuilder builder = new StringBuilder();
                builder.Append(readings.timestamp.ToLocalTime().ToString()+",");
                foreach(var reading in readings.readings) {
                    builder.Append($"{reading.value},");
                }
                lines.Add(builder.ToString());
            }
            Console.WriteLine("Writing Out Data");
            await File.WriteAllLinesAsync(fileName, lines);
            Console.WriteLine("Check File");
        }
        static async Task WriteOutAlertsFile(string deviceName, DateTime start, DateTime stop, string fileName) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceName + "_data");

            var alertItems = database.GetCollection<MonitorAlert>("alert_items").Find(_ => true).ToList();
            var alertReadings = database.GetCollection<AlertReadings>("alert_readings");
            Console.WriteLine("Starting query");
            var aReadings = await (await alertReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop)).ToListAsync();
            //var aReadings = await (await analogReadings.FindAsync(_=>true)).ToListAsync();
            var headers = alertItems.Select(e => e.displayName).ToList();
            StringBuilder hbuilder = new StringBuilder();
            hbuilder.Append("timestamp,");
            headers.ForEach((id) => {
                hbuilder.Append($"{id},");
            });
            Console.WriteLine($"Query Completed.  Count: {aReadings.Count()}");
            List<string> lines = new List<string>();
            lines.Add(hbuilder.ToString());
            foreach (var readings in aReadings) {
                StringBuilder builder = new StringBuilder();
                builder.Append(readings.timestamp.ToLocalTime().ToString() + ",");
                foreach (var reading in readings.readings) {
                    builder.Append($"{reading.reading},");
                }
                lines.Add(builder.ToString());
            }
            Console.WriteLine("Writing Out Data");
            File.WriteAllLines(fileName, lines);
            Console.WriteLine("Check File");
        }*/
    }
}
