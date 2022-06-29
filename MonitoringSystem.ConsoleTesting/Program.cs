using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringSystem.Shared.Data;
using MonitoringData.Infrastructure.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using MonitoringData.Infrastructure.Services.DataAccess;
using System.Diagnostics;
using MonitoringData.Infrastructure.Services.DataLogging;
using MonitoringSystem.Shared.Services;

namespace MonitoringSystem.ConsoleTesting {
    public record class Test {
        public int Value { get; set; }
    }

    public class AnalogReadingDto {
        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }
        public float Value { get; set; }
    }

    public class Program {
        static readonly CancellationTokenSource s_cts = new CancellationTokenSource();
        static async Task Main(string[] args) {
            //await BuildEmailSettingsCollection();
            //await CheckEmailSettigns();
            //await BuildSettingsDB();
            // await CheckSettings();
            //await WriteOutAnalogFile("gasbay", new DateTime(2022, 6, 16, 0, 0, 0), DateTime.Now, @"C:\MonitorFiles\gasbayanalogt3.csv");
            //await WriteOutAlertsFile("gasbay", new DateTime(2022, 6, 13, 0, 0, 0), DateTime.Now,@"C:\MonitorFiles\gasbayalerts.csv");
            //Stopwatch watch = new Stopwatch();         
            //Console.WriteLine("Done");
            //Console.ReadKey();
            /*using var context = new FacilityContext();
            var gasbay = await context.Devices.OfType<ModbusDevice>().FirstOrDefaultAsync(e => e.Identifier == "epi1");*/


            /*SmtpClient client = new SmtpClient();
            await client.ConnectAsync("192.168.0.123",25,false);
            MimeMessage mailMessage = new MimeMessage();
            BodyBuilder bodyBuilder = new BodyBuilder();
            mailMessage.To.Add(new MailboxAddress("Andrew Elmendorf","aelmendorf@s-et.com"));
            mailMessage.From.Add(new MailboxAddress("Monitor Alerts","monitorAlerts@s-et.com"));

            MessageBuilder builder = new MessageBuilder();
            builder.StartMessage("A Test");
            builder.AppendAlert("Alert 2","Alarm","50");
            builder.AppendStatus("Alert 1","Okay","0");
            builder.AppendStatus("Alert 2","Alarm","55");
            
            bodyBuilder.HtmlBody=builder.FinishMessage();
            mailMessage.Body = bodyBuilder.ToMessageBody();
            await client.SendAsync(mailMessage);*/

            //await emailService.SendMessageAsync("Test",message);
            //MailMessage mailMessage = new MailMessage(from.Address,"aelmendorf@s-et.com,rakesh@s-et.com,bmurdaugh@s-et.com,achapman@s-et.com","Test Smtp Alert Email",message);
            //mailMessage.IsBodyHtml = true;
            //mailMessage.


            //await client.SendMailAsync()
            //Console.WriteLine("Message sent");
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
            /*await UpdateChannelSensor(166, 6);
            await UpdateChannelSensor(167, 6);
            await UpdateChannelSensor(168, 8);
            await UpdateChannelSensor(169, 8);
            await UpdateChannelSensor(170, 7);
            await UpdateChannelSensor(171, 7);*/
            //await BuildSensorCollection();
            //await UpdateAnalogSensor();
            /*var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("epi1_data");
            var analogItems = await database.GetCollection<AnalogChannel>("analog_items").Find(_=>true).ToListAsync();
            foreach (var item in analogItems) {
                Console.WriteLine($"Name: {item.identifier} SensorId: {item.sensorId}");
            }*/
        }

        static async Task UpdateAnalogSensor() {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("epi1_data");
            var analogItems = database.GetCollection<AnalogChannel>("analog_items");
            
            var filter1 = Builders<AnalogChannel>.Filter.Where(e => e.identifier.Contains("H2"));
            var update1 = Builders<AnalogChannel>.Update.Set(e => e.sensorId, 1);
            
            var filter2 = Builders<AnalogChannel>.Filter.Where(e => e.identifier.Contains("O2"));
            var update2 = Builders<AnalogChannel>.Update.Set(e => e.sensorId, 2);
            
            var filter3 = Builders<AnalogChannel>.Filter.Where(e => e.identifier.Contains("NH3"));
            var update3 = Builders<AnalogChannel>.Update.Set(e => e.sensorId, 3);
            
            var filter4 = Builders<AnalogChannel>.Filter.Where(e => e.identifier.Contains("N2"));
            var update4 = Builders<AnalogChannel>.Update.Set(e => e.sensorId, 4);
            
            var filter5 = Builders<AnalogChannel>.Filter.Where(e => e.identifier.Contains("H2 LEL"));
            var update5 = Builders<AnalogChannel>.Update.Set(e => e.sensorId, 5);
            
            var filter6 = Builders<AnalogChannel>.Filter.Where(e => e.identifier.Contains("Weight"));
            var update6 = Builders<AnalogChannel>.Update.Set(e => e.sensorId, 6);
            
            var filter7 = Builders<AnalogChannel>.Filter.Where(e => e.identifier.Contains("Temp."));
            var update7 = Builders<AnalogChannel>.Update.Set(e => e.sensorId, 8);
            
            var filter8 = Builders<AnalogChannel>.Filter.Where(e => e.identifier.Contains("Cycle"));
            var update8 = Builders<AnalogChannel>.Update.Set(e => e.sensorId, 7);
            
            var filter9 = Builders<AnalogChannel>.Filter.Where(e => e.identifier.Contains("Analog"));
            var update9 = Builders<AnalogChannel>.Update.Set(e => e.sensorId, 0);
            
            await analogItems.UpdateManyAsync(filter1, update1);
            await analogItems.UpdateManyAsync(filter2, update2);
            await analogItems.UpdateManyAsync(filter3, update3);
            await analogItems.UpdateManyAsync(filter4, update4);
            await analogItems.UpdateManyAsync(filter5, update5);
            await analogItems.UpdateManyAsync(filter6, update6);
            await analogItems.UpdateManyAsync(filter7, update7);
            await analogItems.UpdateManyAsync(filter8, update8);
            await analogItems.UpdateManyAsync(filter9, update9);
            Console.WriteLine("Done, Check Database");
        }

        static async Task BuildSensorCollection() {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("monitor_settings");
            var sensorCol = database.GetCollection<SensorType>("sensor_types");
            using var context = new FacilityContext();
            var sensors = await context.Sensors.Select(e=>new SensorType() {
                _id = e.Id,
                Name = e.Name,
                Units=e.Units,
                YAxisStart = e.YAxisMin,
                YAxisStop = e.YAxitMax
            }).ToListAsync();
            await sensorCol.InsertManyAsync(sensors);
            Console.WriteLine("Done,Check Database");
        }

        static async Task UpdateChannelSensor(int channelId,int sensorId) {
            using var context = new FacilityContext();
            var channel = await context.Channels.OfType<AnalogInput>().Include(e => e.Sensor)
                .FirstOrDefaultAsync(e => e.Id == channelId);
            var sensor = await context.Sensors.FirstOrDefaultAsync(e => e.Id == sensorId);
            channel.Sensor = sensor;
            context.Update(channel);
            context.Update(sensor);
            await context.SaveChangesAsync();
            Console.WriteLine($"{channel.Identifier} updated with {sensor.Name}, check database");
        }
        
        static async Task BuildSettingsDB() { 
            var context = new FacilityContext();
            
            var devices = context.Devices.ToList();

            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("monitor_settings");
            //await database.CreateCollectionAsync("monitor_devices");
            var monitorDevCollection = database.GetCollection<ManagedDevice>("monitor_devices");

            List<ManagedDevice> monitorDevices = new List<ManagedDevice>();
            foreach (var device in devices) {
                ManagedDevice dev = new ManagedDevice();
                dev.DatabaseName = device.DatabaseName;
                dev.DeviceName = device.Identifier;
                dev.DeviceType = device.GetType().Name;
                dev.HubAddress = device.HubName;
                monitorDevices.Add(dev);
            }

            await monitorDevCollection.InsertManyAsync(monitorDevices);
            Console.WriteLine("Check Database");
            Console.ReadKey();
        }
        
        static async Task BuildEmailSettingsCollection() { 
            var context = new FacilityContext();
            
            var devices = context.Devices.ToList();

            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("monitor_settings");
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
            await emailCollection.InsertManyAsync(recipients);
            Console.WriteLine("Check Database");
            Console.ReadKey();
        }

        static async Task CheckEmailSettigns() {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("monitor_settings");
            var recipients = await database.GetCollection<EmailRecipient>("email_recipients").Find(_ => true)
                .ToListAsync();
            Console.WriteLine("Recipients: ");
            foreach (var recipient in recipients) {
                Console.WriteLine($"User: {recipient.Username} Email: {recipient.Address}");
            }

            Console.WriteLine("Press Any key to exit");
            Console.ReadKey();
        }

        static async Task CheckSettings() {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("monitor_settings");
            //await database.CreateCollectionAsync("monitor_devices");
            var monitorDevCollection = database.GetCollection<ManagedDevice>("monitor_devices");
            var monitorDevices = await monitorDevCollection.Find(_ => true).ToListAsync();
            foreach (var device in monitorDevices) {
                Console.WriteLine(device.DeviceName);
            }

            Console.WriteLine("Done");
        }

        static async Task BenchmarkOld() {
            var client = new MongoClient("mongodb://172.20.3.41");
            var dbOld = client.GetDatabase("epi1_data");
            var aCollection = dbOld.GetCollection<AnalogChannel>("analog_items");
            var aItems = await aCollection.Find(_ => true).ToListAsync();
            var aCollectionOld = dbOld.GetCollection<AnalogReadings>("analog_readings");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var oldReadings =await aCollectionOld.Find(e =>
                e.timestamp >= new DateTime(2022, 5, 27) && e.timestamp <= new DateTime(2022, 6, 6))
                .ToListAsync();
            List<AnalogReadingDto> readings = new List<AnalogReadingDto>();
            foreach (var reading in oldReadings) {
                foreach (var read in reading.readings) {
                    var temp= aItems.FirstOrDefault(e => e._id == read.itemid);
                    readings.Add(new AnalogReadingDto() {
                        Name=temp?.identifier,
                        TimeStamp = reading.timestamp,
                        Value = (float)read.value
                    });
                }
            }
            watch.Stop();
            Console.WriteLine($"Old: {watch.ElapsedMilliseconds}");
            Console.WriteLine($"Old Count: {oldReadings.Count()}");
            Console.WriteLine($"Old After: {readings.Count() / 16}");
        }
        static async Task BenchmarkNew() {
            var client = new MongoClient("mongodb://172.20.3.41");
            var dbNew = client.GetDatabase("epi1_data_temp");
            var aCollectionNew = dbNew.GetCollection<AnalogReadingV2>("analog_readings");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var newReadings =await aCollectionNew.Find(e =>
                    e.timestamp >= new DateTime(2022, 5, 27) && e.timestamp <= new DateTime(2022, 6, 6))
                .ToListAsync();
            watch.Stop();
            Console.WriteLine($"New: {watch.ElapsedMilliseconds}");
            Console.WriteLine($"New Count: {newReadings.Count() / 16}");
        }
        
        static async Task SyncDb(string dbOld,string dbNew) {
            Console.WriteLine("Configuring Update");
            var client = new MongoClient("mongodb://172.20.3.41");
            var db = client.GetDatabase(dbOld);
            var newDb = client.GetDatabase(dbNew);
            var oldAnalogItems = db.GetCollection<AnalogChannel>("analog_items")
                .Find(_=>true).ToList();
            var newAnalogItems = newDb.GetCollection<AnalogChannel>("analog_items");
            foreach (var oldAItem in oldAnalogItems) {
                var filter = Builders<AnalogChannel>.Filter.Eq(e => e._id, oldAItem._id);
                await newAnalogItems.UpdateOneAsync(filter, BuildUpdate(oldAItem));
            }
            Console.WriteLine("Should be done,check database");
        }

        static async Task SyncReadings(string dbOld,string dbNew) {
            Console.WriteLine("Configuring Reading Sync");
            var client = new MongoClient("mongodb://172.20.3.41");
            var db = client.GetDatabase(dbOld);
            var newDb = client.GetDatabase(dbNew);
            var oldAnalogItems = db.GetCollection<AnalogChannel>("analog_items")
                .Find(_=>true).ToList();
            var newAnalogReadings = newDb.GetCollection<AnalogReadingV2>("analog_readings");
            Console.WriteLine("Fetching old readings,please wait");
            var oldAnalogReadings = await db.GetCollection<AnalogReadings>("analog_readings")
                .Find(e=>e.timestamp>=new DateTime(2022,5,26) && e.timestamp<=DateTime.Now)
                .ToListAsync();
            var newAnalogItems = db.GetCollection<AnalogChannel>("analog_items");
            foreach (var oldReading in oldAnalogReadings) {
                List<AnalogReadingV2> newReadings = new List<AnalogReadingV2>();
                foreach (var reading in oldReading.readings) {
                    newReadings.Add(new AnalogReadingV2(){itemid = reading.itemid,value = reading.value,timestamp = oldReading.timestamp});
                }
                await newAnalogReadings.InsertManyAsync(newReadings);
            }
            Console.WriteLine("Should be done, check database");
            Console.ReadKey();
        }

        static UpdateDefinition<AnalogChannel> BuildUpdate(AnalogChannel channel) {
            var update = Builders<AnalogChannel>.Update
                .Set(e => e.identifier, channel.identifier)
                .Set(e => e.display, channel.display)
                .Set(e => e.recordThreshold, channel.recordThreshold)
                .Set(e => e.factor, channel.factor)
                .Set(e => e.reg, channel.reg)
                .Set(e => e.reglen, channel.reglen)
                .Set(e => e.l1action, channel.l1action)
                .Set(e => e.l2action, channel.l2action)
                .Set(e => e.l3action, channel.l3action)
                .Set(e => e.l1setpoint, channel.l1setpoint)
                .Set(e => e.l2setpoint, channel.l2setpoint)
                .Set(e => e.l3setpoint, channel.l3setpoint)
                .Set(e => e.sensorId, channel.sensorId);
            return update;
        }
        
       public static async Task CreateMongoDB(string deviceName) {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.41");
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .Select(e => new DeviceDto() { Identifier = e.Identifier, NetworkConfiguration = e.NetworkConfiguration })
                .FirstOrDefault(e => e.Identifier == deviceName);

            if(device is not null) {
                Console.WriteLine($"Device {device.Identifier} found");
                var database = client.GetDatabase($"{device.Identifier.ToLower()}_data_temp");
                Console.WriteLine("Creating Collections");
                Console.WriteLine($"Device {device.Identifier.ToLower()} found");

                Console.WriteLine("Creating Collections");
                await database.CreateCollectionAsync("analog_items");
                await database.CreateCollectionAsync("discrete_items");
                await database.CreateCollectionAsync("output_items");
                await database.CreateCollectionAsync("virtual_items");
                await database.CreateCollectionAsync("action_items");
                await database.CreateCollectionAsync("alert_items");
                await database.CreateCollectionAsync("device_items");

                await database.CreateCollectionAsync("analog_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp","itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("discrete_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid",granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("virtual_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp","itemid", granularity: TimeSeriesGranularity.Seconds)
                    });
                
                await database.CreateCollectionAsync("alert_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid",granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("device_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                Console.WriteLine("Collections Created, Populating configurations");

                IMongoCollection<MonitorDevice> deviceItems = database.GetCollection<MonitorDevice>("device_items");
                MonitorDevice deviceConfig = new MonitorDevice();
                deviceConfig.Created = DateTime.Now;
                deviceConfig.identifier = device.Identifier;
                deviceConfig.NetworkConfiguration = device.NetworkConfiguration;

                IMongoCollection<AnalogChannel> analogItems = database.GetCollection<AnalogChannel>("analog_items");
                var analogChannels = await context.Channels.OfType<AnalogInput>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Include(e => e.Alert)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new AnalogChannel() {
                        identifier = e.DisplayName,
                        _id = e.Id,
                        factor = 1,
                        display = e.Display,
                        l1action=ActionType.SoftWarn,
                        l2action=ActionType.Warning,
                        l3action=ActionType.Alarm,
                        reg=e.ModbusAddress.Address,
                        reglen=e.ModbusAddress.RegisterLength
                    }).ToListAsync();

                var discreteItems = database.GetCollection<DiscreteChannel>("discrete_items");
                var discreteChannels = await context.Channels.OfType<DiscreteInput>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
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
                    }).ToListAsync();

                await deviceItems.InsertOneAsync(deviceConfig);
                await analogItems.InsertManyAsync(analogChannels);
                await discreteItems.InsertManyAsync(discreteChannels);
                await outputItems.InsertManyAsync(outputs);
                await actionItems.InsertManyAsync(actions);
                await monitorAlerts.InsertManyAsync(alerts);
                await virtualItems.InsertManyAsync(virtualChannels);
                Console.WriteLine("Check database, press any key to exit");
            } else {
                Console.WriteLine("Error: Device not found");
            }
            Console.ReadKey();
        }

        static async Task WriteOutAnalogFile(string deviceName, DateTime start, DateTime stop, string fileName) {
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
        }

        public static async Task UpdateChannels(string deviceName) {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceName+"_data");
            var deviceItems = database.GetCollection<MonitorDevice>("device_items");
            var analogItems = database.GetCollection<AnalogChannel>("analog_items");
            var discreteItems = database.GetCollection<DiscreteChannel>("discrete_items");
            var virtualItems = database.GetCollection<VirtualChannel>("virtual_items");

            var analogChannels = await context.Channels.OfType<AnalogInput>()
                .AsNoTracking()
                .Include(e => e.ModbusDevice)
                .Include(e=>e.Sensor)
                .Where(e => e.ModbusDevice.Identifier == deviceName)
                .ToListAsync();

            var discreteChannels = await context.Channels.OfType<DiscreteInput>()
                .AsNoTracking()
                .Include(e => e.ModbusDevice)
                .Where(e => e.ModbusDevice.Identifier == deviceName)
                .ToListAsync();

            var virtualChannels = await context.Channels.OfType<VirtualInput>()
                .AsNoTracking()
                .Include(e => e.ModbusDevice)
                .Where(e => e.ModbusDevice.Identifier == deviceName)
                .ToListAsync();

            var latest = deviceItems.AsQueryable().Where(_ => true).Max(e => (DateTime?)e.Created);
            if (latest is not null) {
                var update = Builders<MonitorDevice>.Update.Set(e => e.recordInterval, 60);
                await deviceItems.UpdateOneAsync<MonitorDevice>(e => e.Created == latest, update);
            }

            foreach (var channel in analogChannels) {
                double threshold = 10000.00;
                if (channel.Sensor != null) {
                    threshold = channel.Sensor.RecordThreshold;
                }
                var update = Builders<AnalogChannel>.Update
                    .Set(e => e.factor, 10)
                    .Set(e => e.display, channel.Connected)
                    .Set(e => e.identifier, channel.DisplayName)
                    .Set(e => e.recordThreshold, threshold);
                await analogItems.UpdateOneAsync(e=>e._id==channel.Id,update);
            }

            foreach (var channel in discreteChannels) {
                var update = Builders<DiscreteChannel>.Update
                    .Set(e => e.display, channel.Connected)
                    .Set(e => e.identifier, channel.DisplayName);
                await discreteItems.UpdateOneAsync(e => e._id == channel.Id, update);
            }

            foreach (var channel in virtualChannels) {
                var update = Builders<VirtualChannel>.Update
                    .Set(e => e.display, channel.Connected)
                    .Set(e => e.identifier, channel.DisplayName);
                await virtualItems.UpdateOneAsync(e => e._id == channel.Id, update);
            }
            Console.WriteLine("Check Database");
        }

        public static async Task CreateMongoDevice(string deviceName) {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceName+"_data");
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .FirstOrDefault(e => e.Identifier == deviceName);
            if(device is not null) {
                var deviceItems = database.GetCollection<MonitorDevice>("device_items");
                MonitorDevice deviceConfig = new MonitorDevice();
                deviceConfig.Created = DateTime.Now;
                deviceConfig.identifier = device.Identifier;
                deviceConfig.NetworkConfiguration = device.NetworkConfiguration;
                await deviceItems.InsertOneAsync(deviceConfig);
            } else {
                Console.WriteLine("Error: Device not found");
            }
        }

        public static void TestRecordList(IList<Test> modify) {
            for(int i = 0; i < modify.Count; i++) {
                modify[i].Value += 1;
            }
        }

        public static async Task TestAlerts() {
            Dictionary<Type, string> collectionNames = new Dictionary<Type, string>();
            collectionNames.Add(typeof(MonitorAlert), "alert_items");
            collectionNames.Add(typeof(ActionItem), "action_items");
            collectionNames.Add(typeof(AnalogChannel), "analog_items");
            collectionNames.Add(typeof(DiscreteChannel), "discrete_items");
            collectionNames.Add(typeof(VirtualChannel), "virtual_items");
            collectionNames.Add(typeof(OutputItem), "output_items");

            collectionNames.Add(typeof(AnalogReading), "analog_readings");
            collectionNames.Add(typeof(DiscreteReading), "discrete_readings");
            collectionNames.Add(typeof(VirtualReading), "virtual_readings");
            collectionNames.Add(typeof(OutputReading), "output_readings");
            collectionNames.Add(typeof(AlertReading), "alert_readings");
            collectionNames.Add(typeof(ActionReading), "action_readings");

            collectionNames.Add(typeof(DeviceReading), "device_readings");
            collectionNames.Add(typeof(MonitorDevice), "device_items");

            var repo = new MonitorDataService("mongodb://172.20.3.30","epi1_data_test", collectionNames);
            var alertService = new AlertService("mongodb://172.20.3.30", "epi1_data_test", collectionNames[typeof(ActionItem)], collectionNames[typeof(MonitorAlert)]);
            await repo.LoadAsync();
            await alertService.Initialize();
            var itemAlerts = repo.MonitorAlerts.Select(alert => new AlertRecord(alert, ActionType.Okay)).ToList();
            DateTime now = DateTime.Now;
            int count = 0;
            while (true) {
                await alertService.ProcessAlerts(itemAlerts,now);
                var alertRecord=itemAlerts.FirstOrDefault(e => e.ChannelId==48);
                if (count == 0) {
                    alertRecord.CurrentState = ActionType.SoftWarn;
                    alertRecord.ChannelReading = 100.0f;
                    count++;
                } else if (count == 1) {
                    alertRecord.CurrentState = ActionType.Warning;
                    alertRecord.ChannelReading = 500.0f;
                    count++;
                } else {
                    alertRecord.CurrentState = ActionType.Alarm;
                    alertRecord.ChannelReading = 1000.0f;
                    count = 0;
                }
                await Task.Delay(1000);
            }
        }

        public static async Task RunDataLogger() {
            var datalogger = new DataLoggerWrapper();
            await datalogger.StartAsync();
            Console.WriteLine("Press q to exit");
            do {

            } while (Console.ReadKey().Key != ConsoleKey.Q);

            Console.WriteLine("Exiting program");
        }

        static async Task ClearAlert(AlertReading reading,MonitorAlert alert, IMongoCollection<MonitorAlert> alerts) {
            var update = Builders<MonitorAlert>.Update
                .Set(e => e.CurrentState, reading.state)
                .Set(e => e.latched, false);
            await alerts.UpdateOneAsync(e => e._id == alert._id, update);
        }

        //static UpdateDefinition<MonitorAlert> AlertUpdateAlreadyLatch(AlertReading reading) {
        //    return Builders<MonitorAlert>.Update
        //        .Set(e => e.lastAlarm, reading.timestamp)
        //        .Set(e => e.CurrentState, reading.state);
        //}

        //static UpdateDefinition<MonitorAlert> AlertUpdateLatch(AlertReading reading) {
        //    return Builders<MonitorAlert>.Update
        //        .Set(e => e.lastAlarm, reading.timestamp)
        //        .Set(e => e.CurrentState, reading.state)
        //        .Set(e => e.latched, true);
        //}

        public static async Task AlertUpdate(string deviceName) {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase(deviceName+"_data");
            var alertItems = database.GetCollection<MonitorAlert>("alert_items");
            using var context = new FacilityContext();
            var alerts = await context.Alerts
                .Include(e => e.InputChannel)
                .ThenInclude(e => e.ModbusDevice)
                .Where(e => e.InputChannel.ModbusDevice.Identifier == deviceName)
                .ToListAsync();

            alerts.ForEach((alert) => {
                var update = Builders<MonitorAlert>.Update
                .Set(s => s.displayName, alert.DisplayName)
                .Set(s=>s.itemType,alert.AlertItemType)
                .Set(s=>s.enabled,alert.Enabled);
                var monitorAlert = alertItems.FindOneAndUpdate<MonitorAlert>(e => e._id == alert.Id, update);
                //monitorAlert.displayName = alert.DisplayName;
                //alertItems.UpdateOne(monitorAlert);
            });

            Console.WriteLine("Check database");
        }

        public static async Task AlertItemTypeUpdate(string deviceName) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase($"{deviceName}_data");
            var alertItems = database.GetCollection<MonitorAlert>("alert_items");
            using var context = new FacilityContext();
            var alerts = await context.Alerts
                .Include(e => e.InputChannel)
                .ThenInclude(e => e.ModbusDevice)
                .Where(e => e.InputChannel.ModbusDevice.Identifier == deviceName)
                .ToListAsync();
            alerts.ForEach((alert) => {
                var update = Builders<MonitorAlert>.Update.Set(s => s.itemType, alert.AlertItemType);
                var monitorAlert = alertItems.FindOneAndUpdate<MonitorAlert>(e => e._id == alert.Id, update);
                monitorAlert.displayName = alert.DisplayName;
                //alertItems.UpdateOne(monitorAlert);
            });
            Console.WriteLine("Check database");
            Console.ReadKey();
        }

        public static async Task AlertItemUpdateEnabled() {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("epi2_data");
            var alertItems = database.GetCollection<MonitorAlert>("alert_items");
            using var context = new FacilityContext();
            var alerts = await context.Alerts
                .Include(e => e.InputChannel)
                .ThenInclude(e => e.ModbusDevice)
                .Where(e => e.InputChannel.ModbusDevice.Identifier == "epi2")
                .ToListAsync();

            alerts.ForEach((alert) => {
                var update = Builders<MonitorAlert>.Update.Set(s => s.enabled, alert.Enabled);
                var monitorAlert = alertItems.FindOneAndUpdate<MonitorAlert>(e => e._id == alert.Id, update);
                //monitorAlert.displayName = alert.DisplayName;
                //alertItems.UpdateOne(monitorAlert);
            });

            Console.WriteLine("Check database");
        }

        public static void ActionItemUpdate() {
            var client = new MongoClient("mongodb://172.20.3.30");
            var e2Db = client.GetDatabase("epi2_data");
            var e1Db = client.GetDatabase("epi1_data");
            var e2AItems = e2Db.GetCollection<ActionItem>("action_items");
            var e1AItems = e1Db.GetCollection<ActionItem>("action_items");
            Dictionary<int, ActionType> actionTypeMap = new Dictionary<int, ActionType>();
            actionTypeMap.Add(1, ActionType.Okay);
            actionTypeMap.Add(2, ActionType.Alarm);
            actionTypeMap.Add(3, ActionType.Warning);
            actionTypeMap.Add(4, ActionType.SoftWarn);
            actionTypeMap.Add(5, ActionType.Maintenance);
            actionTypeMap.Add(6, ActionType.Custom);
            foreach(var item in actionTypeMap) {
                var update = Builders<ActionItem>.Update.Set(s => s.actionType, item.Value);
                var e2Item = e2AItems.FindOneAndUpdate(e => e._id == item.Key, update);
                var e1Item = e1AItems.FindOneAndUpdate(e => e._id == item.Key, update);
            }
            Console.WriteLine("Check Database");
        }

        public static void UpdateActionEmailSettings() {
            using var context = new FacilityContext();
            var actions = context.FacilityActions.ToList();
            var client = new MongoClient("mongodb://172.20.3.30");
            var epi1Db = client.GetDatabase("epi1_data");
            var epi2Db = client.GetDatabase("epi2_data");
            var e1Actions = epi1Db.GetCollection<ActionItem>("action_items");
            var e2Actions = epi2Db.GetCollection<ActionItem>("action_items");


            actions.ForEach((act) => {
                var update = Builders<ActionItem>.Update
                .Set(e => e.EmailEnabled, act.EmailEnabled)
                .Set(e => e.EmailPeriod, act.EmailPeriod);
                e1Actions.FindOneAndUpdate(e => e.actionType == act.ActionType, update);
                e2Actions.FindOneAndUpdate(e => e.actionType == act.ActionType, update);
            });
            Console.WriteLine("Check Databases");
        }

        static async Task CreateConfigDatabase(string deviceName) {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.41");
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .Select(e => new DeviceDto() { 
                    Identifier = e.Identifier, 
                    NetworkConfiguration = e.NetworkConfiguration 
                }).FirstOrDefault(e => e.Identifier == deviceName);

            if (device != null) {
                Console.WriteLine($"Device {device.Identifier.ToLower()} found");
                var database = client.GetDatabase($"{device.Identifier.ToLower()}_data_test");

                Console.WriteLine("Creating Collections");
                await database.CreateCollectionAsync("analog_items");
                await database.CreateCollectionAsync("discrete_items");
                await database.CreateCollectionAsync("output_items");
                await database.CreateCollectionAsync("virtual_items");
                await database.CreateCollectionAsync("action_items");
                await database.CreateCollectionAsync("alert_items");
                await database.CreateCollectionAsync("device_items");

                IMongoCollection<MonitorDevice> deviceItems = database.GetCollection<MonitorDevice>("device_items");
                MonitorDevice deviceConfig = new MonitorDevice();
                deviceConfig.Created = DateTime.Now;
                deviceConfig.identifier = device.Identifier;
                deviceConfig.NetworkConfiguration = device.NetworkConfiguration;

                IMongoCollection<AnalogChannel> analogItems = database.GetCollection<AnalogChannel>("analog_items");
                var analogChannels = await context.Channels.OfType<AnalogInput>()
                    .AsNoTracking()
                    .Include(e=>e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e=>e.SystemChannel)
                    .Select(e=>new AnalogChannel() { 
                        identifier=e.DisplayName
                        ,_id=e.Id,
                        factor=10,
                        display=e.Display
                    }).ToListAsync();

                var discreteItems = database.GetCollection<DiscreteChannel>("discrete_items");
                var discreteChannels = await context.Channels.OfType<DiscreteInput>()
                    .AsNoTracking()
                    .Include(e=>e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e=>e.SystemChannel)
                    .Select(e => new DiscreteChannel() { 
                        identifier = e.DisplayName,
                        _id = e.Id,
                        display=e.Display
                    }).ToListAsync();

                IMongoCollection<OutputItem> outputItems = database.GetCollection<OutputItem>("output_items");
                var outputs = await context.Channels.OfType<OutputChannel>()
                    .AsNoTracking()
                    .Include(e => e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e=>e.SystemChannel)
                    .Select(e => new OutputItem() { 
                        identifier = e.DisplayName, 
                        _id = e.Id,
                        display=e.Display
                    }).ToListAsync();

                IMongoCollection<ActionItem> actionItems = database.GetCollection<ActionItem>("action_items");
                var actions = await context.FacilityActions
                    .AsNoTracking()
                    .Select(e => new ActionItem() { 
                        identifier = e.ActionName, 
                        _id = e.Id,
                        EmailEnabled=e.EmailEnabled,
                        EmailPeriod=e.EmailPeriod,
                        actionType=e.ActionType,
                        display=true
                    }).ToListAsync();

                IMongoCollection<VirtualChannel> virtualItems = database.GetCollection<VirtualChannel>("virtual_items");
                var virtualChannels = await context.Channels.OfType<VirtualInput>()
                    .AsNoTracking()
                    .Include(e=>e.ModbusDevice)
                    .Where(e => e.ModbusDevice.Identifier == device.Identifier)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new VirtualChannel() { 
                        identifier = e.DisplayName, 
                        _id = e.Id,
                        display=e.Display
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
                        displayName=e.DisplayName,
                        bypassResetTime = e.BypassResetTime,
                        itemType=e.AlertItemType,
                        latched = false,
                        CurrentState = ActionType.Okay 
                    }).ToListAsync();

                await deviceItems.InsertOneAsync(deviceConfig);
                await analogItems.InsertManyAsync(analogChannels);
                await discreteItems.InsertManyAsync(discreteChannels);
                await outputItems.InsertManyAsync(outputs);
                await actionItems.InsertManyAsync(actions);
                await monitorAlerts.InsertManyAsync(alerts);
                await virtualItems.InsertManyAsync(virtualChannels);
                
                Console.WriteLine("Check database, press any key to exit");
            } else {
                Console.WriteLine("Device Not Found, Please Check Identifier");
            }
            Console.ReadKey();
        }
        static async Task CreateReadingsDatabase(string deviceName) {
            using var context = new FacilityContext();
            var client = new MongoClient("mongodb://172.20.3.30");
            var device = context.Devices.OfType<ModbusDevice>()
                .AsNoTracking()
                .Select(e => new DeviceDto() { Identifier = e.Identifier, NetworkConfiguration = e.NetworkConfiguration})
                .FirstOrDefault(e => e.Identifier == deviceName);

            if (device != null) {
                Console.WriteLine($"Device {device.Identifier} found");
                var database = client.GetDatabase($"{device.Identifier.ToLower()}_data_test");
                Console.WriteLine("Creating Collections");

                await database.CreateCollectionAsync("analog_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("discrete_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("output_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("virtual_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("action_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("alert_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                await database.CreateCollectionAsync("device_readings",
                    new CreateCollectionOptions() {
                        TimeSeriesOptions = new TimeSeriesOptions("timestamp", "itemid", granularity: TimeSeriesGranularity.Seconds)
                    });

                Console.WriteLine("Gettting Collections");
                var analogReadings = database.GetCollection<AnalogReading>("analog_readings");
                var discreteReadings = database.GetCollection<DiscreteReading>("discrete_readings");
                var outputReadings = database.GetCollection<OutputReading>("output_readings");
                var virtualReadings = database.GetCollection<VirtualReading>("virtual_readings");
                var actionReadings = database.GetCollection<ActionReading>("action_readings");
                var alertReadings = database.GetCollection<AlertReading>("alert_readings");
                var deviceReadings = database.GetCollection<DeviceReading>("device_readings");

                Console.WriteLine("Creating TimeSeries Indices");

                analogReadings.Indexes.CreateOne(new CreateIndexModel<AnalogReading>(Builders<AnalogReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                discreteReadings.Indexes.CreateOne(new CreateIndexModel<DiscreteReading>(Builders<DiscreteReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                outputReadings.Indexes.CreateOne(new CreateIndexModel<OutputReading>(Builders<OutputReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                virtualReadings.Indexes.CreateOne(new CreateIndexModel<VirtualReading>(Builders<VirtualReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                alertReadings.Indexes.CreateOne(new CreateIndexModel<AlertReading>(Builders<AlertReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                actionReadings.Indexes.CreateOne(new CreateIndexModel<ActionReading>(Builders<ActionReading>.IndexKeys.Ascending(x => x.itemid),
                    new CreateIndexOptions()),
                    new CreateOneIndexOptions());

                deviceReadings.Indexes.CreateOne(new CreateIndexModel<DeviceReading>(Builders<DeviceReading>.IndexKeys.Ascending(x => x.itemid),
                new CreateIndexOptions()),
                new CreateOneIndexOptions());


                Console.WriteLine("Inserting Initialization Data");


                Console.WriteLine("Should be done, check database");
            } else {
                Console.WriteLine("Error: Device not found");
            }

        } 
        static async Task TestReading() {
            using var context = new FacilityContext();
            var device = await context.Devices.AsNoTracking()
                .OfType<ModbusDevice>()
                .FirstOrDefaultAsync(e => e.Identifier == "epi2");
            if (device != null) {
                var client = new MongoClient("mongodb://172.20.3.30");
                var database = client.GetDatabase("epi2_data");

                Console.WriteLine("Device found, setting up read");
                var networkConfig = device.NetworkConfiguration;
                var modbusConfig = networkConfig.ModbusConfig;
                var channelMapping = modbusConfig.ChannelMapping;
                var modbusService = new ModbusService();
                var result = await modbusService.Read(networkConfig.IPAddress, networkConfig.Port, networkConfig.ModbusConfig);
                var discreteInputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.DiscreteStart, (channelMapping.DiscreteStop - channelMapping.DiscreteStart) + 1).ToArray();
                var outputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.OutputStart, (channelMapping.OutputStop - channelMapping.OutputStart) + 1).ToArray();
                var actions = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.ActionStart, (channelMapping.ActionStop - channelMapping.ActionStart) + 1).ToArray();
                var analogInputs = new ArraySegment<ushort>(result.InputRegisters, channelMapping.AnalogStart, (channelMapping.AnalogStop - channelMapping.AnalogStart) + 1).ToArray();
                var alerts = new ArraySegment<ushort>(result.HoldingRegisters, channelMapping.AlertStart, (channelMapping.AlertStop - channelMapping.AlertStart) + 1).ToArray();
                var virts = new ArraySegment<bool>(result.Coils, channelMapping.VirtualStart, (channelMapping.VirtualStop - channelMapping.VirtualStart) + 1).ToArray();

                //Get Configurations and readings collections
                var analogItems = database.GetCollection<AnalogChannel>("analog_items");
                var discreteItems = database.GetCollection<DiscreteChannel>("discrete_items");
                var outputItems = database.GetCollection<OutputItem>("output_items");
                var virtualItems = database.GetCollection<VirtualChannel>("virtual_items");
                var actionItems = database.GetCollection<ActionItem>("action_items");
                var alertItems = database.GetCollection<MonitorAlert>("alert_items");

                var analogReadings = database.GetCollection<AnalogReading>("analog_readings");
                var discreteReadings = database.GetCollection<DiscreteReading>("discrete_readings");
                var outputReadings = database.GetCollection<OutputReading>("analog_readings");
                var virtualReadings = database.GetCollection<VirtualReading>("analog_readings");
                var actionReadings = database.GetCollection<ActionReading>("analog_readings");
                var alertReadings = database.GetCollection<AlertReading>("analog_readings");

                var analogConfig = await analogItems.Find(_=>true).ToListAsync();
                var discreteConfig = await analogItems.Find(_=> true).ToListAsync();
                var outputConfig = await analogItems.Find(_=> true).ToListAsync();
                var virtualConfig = await analogItems.Find(_=> true).ToListAsync();
                var actionConfig = await analogItems.Find(_=> true).ToListAsync();
                var alertConfig = await alertItems.Find(_=> true).ToListAsync();

                var now = DateTime.Now;

                List<AnalogReading> aTempReadings = new List<AnalogReading>();
                for(int i = 0; i < analogInputs.Length; i++) {
                    aTempReadings.Add(new AnalogReading() { itemid = analogConfig[i]._id, value = analogInputs[i] });
                }
                await analogReadings.InsertManyAsync(aTempReadings);
                Console.WriteLine("Done: Check Analog Readings");

            } else {
                Console.WriteLine("Error: Device not found");
            }
        }
    }
    public class DataLoggerWrapper {
        private IDataLogger _dataLogger;
        private Timer _timer;
        
        public DataLoggerWrapper() {
            Dictionary<Type, string> collectionNames = new Dictionary<Type, string>();
            collectionNames.Add(typeof(MonitorAlert), "alert_items");
            collectionNames.Add(typeof(ActionItem), "action_items");
            collectionNames.Add(typeof(AnalogChannel), "analog_items");
            collectionNames.Add(typeof(DiscreteChannel), "discrete_items");
            collectionNames.Add(typeof(VirtualChannel), "virtual_items");
            collectionNames.Add(typeof(OutputItem), "output_items");

            collectionNames.Add(typeof(AnalogReading), "analog_readings");
            collectionNames.Add(typeof(DiscreteReading), "discrete_readings");
            collectionNames.Add(typeof(VirtualReading), "virtual_readings");
            collectionNames.Add(typeof(OutputReading), "output_readings");
            collectionNames.Add(typeof(AlertReading), "alert_readings");
            collectionNames.Add(typeof(ActionReading), "action_readings");

            collectionNames.Add(typeof(DeviceReading), "device_readings");
            collectionNames.Add(typeof(MonitorDevice), "device_items");
            this._dataLogger = new ModbusLogger("mongodb://172.20.3.41", "nh3_data",collectionNames);
        }
        public async Task StartAsync() {
            Console.WriteLine("Starting Logging Service");
            await this._dataLogger.Load();
            this._timer = new Timer(this.DataLogCallback, null,TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }
        public async void DataLogCallback(object state) {
            Console.WriteLine($"{DateTime.Now}: Logged Data");
            await this._dataLogger.Read();
        }
    }
}
