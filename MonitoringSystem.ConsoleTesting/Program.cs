using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using System;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ConsoleTables;
using MailKit.Net.Smtp;
using MimeKit;
using Modbus.Device;
using MongoDB.Bson;
using MonitoringConfig.Data.Model;
using MonitoringData.Infrastructure.Data;
using MonitoringSystem.ConfigApi.Mapping;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Services;
using MonitoringSystem.Shared.Data.SettingsModel;
using MonitoringSystem.Shared.Data.UsageModel;
using ModbusDevice = MonitoringConfig.Data.Model.ModbusDevice;

namespace MonitoringSystem.ConsoleTesting {
    public class Program {
        static async Task Main(string[] args) {
            /*var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("epi1_data_dev");*/
            //await CreateMongoDB("Epi1");
            //await BuildSettingsDB();
            //await BuildEmailSettingsCollection();
            /*HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7133/");

            var response = await client.GetFromJsonAsync<GetDeviceActionsResponse>($"actions/deviceactions/d0d8ae61-6982-429f-de8d-08da81eb1674");
            foreach (var device in response.DeviceActions) {
                Console.WriteLine($"Name: {device.Name}");
            }*/
            /*var context = new MonitorContext();
            var discreteInputs = await context.Channels.OfType<DiscreteInput>()
                .Include(e => e.ModbusDevice)
                .Include(e => e.Alert)
                .ThenInclude(e => ((DiscreteAlert)e).AlertLevel)
                .ThenInclude(e => e.DeviceAction.FacilityAction)
                .Where(e => e.ModbusDevice.Name == "epi1")
                .ToListAsync();
            int count = 0;
            foreach (var input in discreteInputs) {
                if ((input.Alert as DiscreteAlert).AlertLevel.DeviceAction == null) {
                    count++;
                }
            }
            
            Console.WriteLine($"Null Count: {count}");*/
            /*var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("monitor_settings");
            var collection = database.GetCollection<EmailRecipient>("email_recipients");
            await collection.InsertOneAsync(new EmailRecipient() {
                Username = "Norman", Address = "nculbertson@s-et.com", _id = ObjectId.GenerateNewId()
            });
            await collection.InsertOneAsync(new EmailRecipient() {
                Username = "Brandon", Address = "brobinson@s-et.com", _id = ObjectId.GenerateNewId()
            });
            await collection.InsertOneAsync(new EmailRecipient() {
                Username = "Graci", Address = "ghill@s-et.com", _id = ObjectId.GenerateNewId()
            });
            await collection.InsertOneAsync(new EmailRecipient() {
                Username = "Dev", Address = "devendra@s-et.com", _id = ObjectId.GenerateNewId()
            });
            await collection.InsertOneAsync(new EmailRecipient() {
                Username = "Mark", Address = "mgeppert@s-et.com", _id = ObjectId.GenerateNewId()
            });
            Console.WriteLine("Check Database");*/
            /*ModbusService modservice = new ModbusService();
            Console.WriteLine("Epi1 Running");
            await modservice.WriteCoil("172.20.5.39", 502, 1, 2, true);
            await modservice.WriteCoil("172.20.5.39", 502, 1, 0, true);
            await Task.Delay(1000);
            await modservice.WriteCoil("172.20.5.39", 502, 1, 0, false);
            await modservice.WriteCoil("172.20.5.39", 502, 1, 2, false);*/
            /*await WriteOutAnalogFile("epi1", new DateTime(2024, 2, 17), DateTime.Now, @"C:\MonitorFiles\analogreadingsEpi1.csv");*/
            //var client = new MongoClient("mongodb:");*/
            //await RemoteAlertTesting();
            //await TestModbus();
            /*AmmoniaController controller = new AmmoniaController();
            ConsoleTable table = new ConsoleTable("Scale", "ZeroRaw", "NonZeroRaw", "Zero", "NonZero", "Combined",
                "Tare", "GasWeight","CurrentWeight");*/
            /*await TestAmmoniaController(1, controller,table);
            await TestAmmoniaController(2, controller,table);
            await TestAmmoniaController(3, controller,table);
            await TestAmmoniaController(4, controller,table);*/
            /*Console.WriteLine(table.ToString());*/

            //await CreateBulkGasSettings();
            //await UpdateBulkSettings();
            //await TestExternalAlertEmail();
            //ExchangeEmailService emailService = new ExchangeEmailService();
            //await emailService.SendMessageAsync("Gas Refill","Nitrogen","74","inH20","Now");
            //await emailService.SendTestMessageAsync();
           //await UpdateBulkEmailSettings();
           //await CreateBulkEmailSettings();
           //await CreateNH3BulkGasSettings();
           //await CreateSIBulkGasSettings();
           //await TestPopList();
           //await WriteOutAnalogFile("epi1", new DateTime(2024, 1,1), DateTime.Now, @"C:\Users\aelmendo\Documents\MonitorFiles\ep1-rakesh.csv");

           List<Testitem> testItems = [];
           Random rand = new Random();
           for (int i = 0; i < 3; i++) {
               testItems.Add(new Testitem() {
                   _id=i,
                   Value=rand.Next(1,100)
               });
           }
           
           Console.WriteLine($"[{string.Join(',', testItems.Select(e=>e.Value))}]");
           
           for (int i = 0; i < 3; i++) {
               var item=testItems.FirstOrDefault(e => e._id == i);
               item.Value=rand.Next(1,100);
           }
           Console.WriteLine($"[{string.Join(',', testItems.Select(e=>e.Value))}]");
           
        }

        static async Task TestExternalEmailCalc(int days=14) {
            IMongoClient client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("epi1_data");
            var analogCollection = database.GetCollection<AnalogItem>("analog_items");
            var analogReadCollection = database.GetCollection<AnalogReadings>("analog_readings");
            var usageCollection = database.GetCollection<UsageDayRecord>("h2_usage");
            var item = await analogCollection.Find(e => e.Identifier == "Bulk H2(PSI)").FirstOrDefaultAsync();
            var date=DateTime.Now.AddDays(-days);
           
            var allUsage=await GetUsageRecordsV2(usageCollection,analogReadCollection,0,item);
            var usage=allUsage.Where(e=>e.Date>=date).ToList();
            var rate = usage.Average(e => e.Usage);
            var lastReadingEntry =  analogReadCollection.AsQueryable()
                .OrderByDescending(r => r.timestamp)
                .FirstOrDefault();
            var lastReading=lastReadingEntry.readings
                .Where(e => e.MonitorItemId == item._id).Select(e => e.Value)
                .FirstOrDefault();
           
            Console.WriteLine($"Avg {days}Day {rate} PPM/Day");

            double suggestedLevel = 550 + (3 * rate);
            Console.WriteLine($"Suggested Level: {suggestedLevel}");
            Console.WriteLine($"SoftWarn: {DateTime.Now.AddDays((lastReading-suggestedLevel)/rate)}))");
           
            //var emailDate = DateTime.Now.AddDays(rate*);
        }

        static async Task SetExternalEmailLevel(AnalogItem item, double level) {
            IMongoClient client = new MongoClient("mongodb://172.20.3.41");
            var settingsCollection=client.GetDatabase("monitor_settings")
                .GetCollection<WebsiteBulkSettings>("bulk_settings");
            var settingsCollectionV2=client.GetDatabase("monitor_settings")
                .GetCollection<WebsiteBulkSettings>("bulk_settings_v2");
            var settings=await settingsCollection.Find(_ => true).FirstOrDefaultAsync();
            /*settings.H2Settings.ExternalEmailLevel = 1000;
            settings.H2Settings.MinExternalEmailLevel=750;*/
            var update = Builders<WebsiteBulkSettings>.Update
                .Set(e => e.H2Settings, settings.H2Settings);
            await settingsCollectionV2.InsertOneAsync(settings);
            Console.WriteLine("Check Database");
        }
        
        static async Task<IEnumerable<UsageDayRecord>> GetUsageRecordsV2(IMongoCollection<UsageDayRecord> usageCollection,
            IMongoCollection<AnalogReadings> analogReadCollection,
            double threshold, AnalogItem? item1, AnalogItem? item2 = null, DateTime? startDate = null) {
            var sort = Builders<UsageDayRecord>.Sort.Descending(e => e.Date);
            var count = await usageCollection.EstimatedDocumentCountAsync();
            if (count == 0) {//new
                var stopDate = DateTime.Now.Date;
                Dictionary<DateTime, List<ValueReturn>> days;
                List<AnalogReadings> readings;
                if (startDate.HasValue) {
                    readings = await analogReadCollection.Find(e => e.timestamp >= startDate && e.timestamp < stopDate)
                        .ToListAsync();
                } else {
                    readings = await analogReadCollection.Find(e => e.timestamp < stopDate)
                        .ToListAsync();
                }
    
                List<ValueReturn> rawData = new List<ValueReturn>();
                if (item2 != null) {
                    foreach (var reading in readings) {
                        var r1 = reading.readings.FirstOrDefault(e => e.MonitorItemId == item1._id)!.Value;
                        var r2 = reading.readings.FirstOrDefault(e => e.MonitorItemId == item2._id)!.Value;
                        var valueReturn = new ValueReturn() {
                            timestamp = reading.timestamp.AddHours(-5), value = (r1 >= 0.00 ? r1 : 0.00) + (r2 >= 0 ? r2 : 0)
                        };
                        rawData.Add(valueReturn);
                    }
                } else {
                    rawData = readings.Select(e => new ValueReturn() {
                        timestamp = e.timestamp.AddHours(-5),
                        value = (e.readings.FirstOrDefault(m => m.MonitorItemId == item1._id)!.Value)
                    }).ToList();
                }
                days = rawData.GroupBy(e =>
                        e.timestamp.Date)
                    .ToDictionary(e => e.Key, e => e.ToList());
                List<UsageDayRecord> dayRecords = new List<UsageDayRecord>();
                foreach (var day in days) {
                    UsageDayRecord usageDayRecord = new UsageDayRecord() {
                        _id = ObjectId.GenerateNewId(),
                        DayOfMonth = day.Key.Day,
                        DayOfWeek = day.Key.DayOfWeek,
                        Date = day.Key,
                        Month = day.Key.Month,
                        WeekOfYear = 
                            CultureInfo.CurrentCulture.DateTimeFormat.Calendar.GetWeekOfYear(day.Key,
                                CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday),
                        Year = day.Key.Year,
                        DayOfYear = day.Key.DayOfYear,
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(day.Key.Month)
                    };
                    usageDayRecord.ChannelIds.Add(item1!._id);
                    if (item2 != null) {
                        usageDayRecord.ChannelIds.Add(item2._id);
                    }
    
                    var first = day.Value[0].value;
                    var last = day.Value[day.Value.Count() - 1].value;
                    var max = day.Value.Max(e=>e.value);
                    var min = day.Value.Min(e => e.value);
    
                    double consumed = 0;
                    if (last < first) {
                        consumed = first - last;
                    } else {
                        consumed = (first - min) + (max - last);
                    }
                    usageDayRecord.Usage = consumed;
                    usageDayRecord.PerHour = consumed/24;
                    usageDayRecord.PerMin = usageDayRecord.PerHour;
                    dayRecords.Add(usageDayRecord);
                }
                await usageCollection.InsertManyAsync(dayRecords);
                return dayRecords.AsEnumerable();
            } else {//Update
                var latest = await usageCollection.Find(_ => true).Sort(sort).FirstAsync();
                var deltaHours = (DateTime.Now - latest.Date.AddDays(1)).TotalHours;
                var now = DateTime.Now.Date;
                if (deltaHours >= 24) {
                    var start = latest.Date.AddDays(1);
                    var stop = DateTime.Now.Date.AddSeconds(-1).AddHours(-5);
                    List<AnalogReadings> readings;
                    Dictionary<DateTime, List<ValueReturn>> days;
                    List<ValueReturn> rawData;
                    if (item2 != null) {
                        readings = await analogReadCollection.Find(e =>
                                e.timestamp >= start &&
                                e.timestamp < stop)
                            .ToListAsync();
                        rawData = readings.Select(e => new ValueReturn() {
                            timestamp = e.timestamp.AddHours(-5),
                            value = (e.readings.FirstOrDefault(m => m.MonitorItemId == item1._id)!.Value +
                                     e.readings.FirstOrDefault(m => m.MonitorItemId == item2._id)!.Value)
                        }).ToList();
    
                    } else {
                        readings = await analogReadCollection.Find(e =>
                                e.timestamp >= start.AddHours(5) &&
                                e.timestamp < stop)
                            .ToListAsync();
                        rawData = readings.Select(e => new ValueReturn() {
                            timestamp = e.timestamp.AddHours(-5),
                            value = (e.readings.FirstOrDefault(m => m.MonitorItemId == item1._id)!.Value)
                        }).ToList();
                    }
                    days = rawData.GroupBy(e =>
                            e.timestamp.Date)
                        .ToDictionary(e => e.Key, e => e.ToList());
                    List<UsageDayRecord> dayRecords = new List<UsageDayRecord>();
                    foreach (var day in days) {
                        UsageDayRecord usageDayRecord = new UsageDayRecord() {
                            _id = ObjectId.GenerateNewId(),
                            DayOfMonth = day.Key.Day,
                            DayOfWeek = day.Key.DayOfWeek,
                            Date = day.Key,
                            Month = day.Key.Month,
                            WeekOfYear =
                                CultureInfo.CurrentCulture.DateTimeFormat.Calendar.GetWeekOfYear(day.Key,
                                    CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday),
                            Year = day.Key.Year,
                            DayOfYear = day.Key.DayOfYear,
                            MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(day.Key.Month)
                        };
                        usageDayRecord.ChannelIds.Add(item1!._id);
                        if (item2 != null) {
                            usageDayRecord.ChannelIds.Add(item2._id);
                        }
                        var first = day.Value.First().value;
                        var last = day.Value.Last().value;
                        var max = day.Value.Max(e=>e.value);
                        var min = day.Value.Min(e => e.value);
    
                        double consumed = 0;
                        if (last < first) {
                            consumed = first - last;
                        } else {
                            consumed = (first - min) + (max - last);
                        }
                        usageDayRecord.Usage = consumed;
                        usageDayRecord.PerHour = consumed/24;
                        usageDayRecord.PerMin = usageDayRecord.PerHour/60;
                        dayRecords.Add(usageDayRecord);
                    }
                    await usageCollection.InsertManyAsync(dayRecords);
                    return dayRecords.AsEnumerable();
                } else {
                    return (await usageCollection.Find(_ => true).ToListAsync()).AsEnumerable();
                }
            }
        }
        
        static Task TestPopList() {
            List<int> values = new List<int>(capacity:5);
            for (int i = 0; i < 10; i++) {
                values.Add(i);
                if (values.Count > 5) {
                    values.RemoveAt(0);
                }
            }

            Console.WriteLine($"Count: {values.Count}");
            foreach (var value in values) {
                Console.WriteLine($"{value}");
            }

            return Task.CompletedTask;
        }

        static async Task UpdateBulkSettings() {
            IMongoClient client = new MongoClient("mongodb://172.20.3.41");
            var e1Database = client.GetDatabase("epi1_data");
            var settingsDatabase = client.GetDatabase("monitor_settings");
            var settingsCollection = settingsDatabase.GetCollection<WebsiteBulkSettings>("bulk_settings");

            var analogCollection = e1Database.GetCollection<AnalogItem>("analog_items");
            var n2Channel = await analogCollection.Find(e => e.Identifier == "Bulk N2(inH20)").FirstOrDefaultAsync();
            var h2Channel = await analogCollection.Find(e => e.Identifier == "Bulk H2(PSI)").FirstOrDefaultAsync();
            
            var settings=await settingsCollection.Find(_ => true)
                .FirstOrDefaultAsync();
            
            var update = Builders<WebsiteBulkSettings>.Update
                .Set(e => e.H2Settings.YAxisMin, 0)
                .Set(e => e.H2Settings.YAxisMax, 3000)
                .Set(e => e.N2Settings.YAxisMin, 0)
                .Set(e => e.N2Settings.YAxisMax, 200);

            await settingsCollection.UpdateOneAsync(e=>e._id==settings._id, update);
            Console.WriteLine("Check Database");
        }
        
        /*static async Task UpdateBulkEmailSettings() {
            IMongoClient client = new MongoClient("mongodb://172.20.3.41");
            var e1Database = client.GetDatabase("epi1_data");
            var settingsDatabase = client.GetDatabase("monitor_settings");
            var settingsCollection = settingsDatabase.GetCollection<WebsiteBulkSettings>("bulk_settings");
            
            var settings=await settingsCollection.Find(_ => true)
                .FirstOrDefaultAsync();
            
            var recp = new List<string>() {
                "ronnie.huffstetler@airgas.com",
                "ANW.Planning.Group@airgas.com"
            };

            var cc = new List<string>() {
                "nculbertson@s-et.com",
                "aelmendorf@s-et.com"
            };

            BulkEmailSettings emailSettings = new BulkEmailSettings();
            emailSettings.ToAddresses = recp;
            emailSettings.CcAddresses = cc;
            emailSettings.Message = "";

            var update = Builders<WebsiteBulkSettings>.Update
                .Set(e => e.EmailSettings, emailSettings);

            await settingsCollection.UpdateOneAsync(e=>e._id==settings._id, update);
            Console.WriteLine("Check Database");
        }*/
        
        static async Task CreateBulkEmailSettings() {
            IMongoClient client = new MongoClient("mongodb://172.20.3.41");
            var settingsDatabase = client.GetDatabase("monitor_settings");
            var settingsCollection = settingsDatabase.GetCollection<BulkEmailSettings>("bulk_email_settings");
            
            var settings=await settingsCollection.Find(_ => true)
                .FirstOrDefaultAsync();
            
            var recp = new List<string>() {
                "ronnie.huffstetler@airgas.com",
                "ANW.Planning.Group@airgas.com"
            };

            var cc = new List<string>() {
                "nculbertson@s-et.com",
                "aelmendorf@s-et.com"
            };

            BulkEmailSettings emailSettings = new BulkEmailSettings();
            emailSettings.ToAddresses = recp;
            emailSettings.CcAddresses = cc;
            emailSettings.Message = "";

            await settingsCollection.InsertOneAsync(emailSettings);
            Console.WriteLine("Check Database");
        }
        static async Task CreateSIBulkGasSettings() {
            IMongoClient client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("epi1_data");
            var settingsDatabase = client.GetDatabase("monitor_settings");
            var settingsCollection = settingsDatabase.GetCollection<WebsiteBulkSettings>("bulk_settings");

            var analogCollection = database.GetCollection<AnalogItem>("analog_items");
            var channel = await analogCollection
                .Find(e => e.Identifier == "Silane")
                .FirstOrDefaultAsync();

            var websiteBulkSettings = await settingsCollection.Find(_ => true).FirstAsync();
                
            BulkGasSettings nh3 = new BulkGasSettings();
            nh3.BulkGasType = BulkGasType.SI;
            nh3.Name = channel.Identifier;
            nh3.ChannelName = "Silane";
            nh3.DeviceName = "epi1_data";
            nh3.EnableAggregation = true;
            nh3.OkayLabel = "Okay";
            nh3.PointColor = KnownColor.Chartreuse;
            nh3.HoursAfter = 6;
            nh3.HoursBefore = 12;
            BulkGasAlert nh3Soft = new BulkGasAlert();
            nh3Soft.Label = ActionType.SoftWarn.ToString();
            nh3Soft.ActionType = ActionType.SoftWarn;
            nh3Soft.Default = true;
            nh3Soft.SetPoint = (int)channel.Level1SetPoint;
            
            BulkGasAlert nh3Warn = new BulkGasAlert();
            nh3Warn.Label = ActionType.Warning.ToString();
            nh3Warn.ActionType = ActionType.Warning;
            nh3Warn.Default = true;
            nh3Warn.SetPoint = (int)channel.Level2SetPoint;

            BulkGasAlert nh3Alarm = new BulkGasAlert();
            nh3Alarm.Label = ActionType.Alarm.ToString();
            nh3Alarm.ActionType = ActionType.Alarm;
            nh3Alarm.Default = true;
            nh3Alarm.SetPoint = (int)channel.Level3SetPoint;

            RefLine nh3AlarmLine = new RefLine();
            nh3AlarmLine.Value = nh3Alarm.SetPoint;
            nh3AlarmLine.Color = KnownColor.Red;
            nh3AlarmLine.Label = "Halt Production";
            
            RefLine nh3WarnLine = new RefLine();
            nh3WarnLine.Value = nh3Warn.SetPoint;
            nh3WarnLine.Color = KnownColor.Yellow;
            nh3WarnLine.Label = "Prepare To Halt";
            
            RefLine nh3SoftLine = new RefLine();
            nh3SoftLine.Value = nh3Soft.SetPoint;
            nh3SoftLine.Color = KnownColor.Wheat;
            nh3SoftLine.Label = "Notify";
            
            
            nh3.SoftAlert=nh3Soft;
            nh3.WarnAlert=nh3Warn;
            nh3.AlrmAlert=nh3Alarm;


            nh3.SoftRefLine = nh3SoftLine;
            nh3.WarnRefLine = nh3WarnLine;
            nh3.AlrmRefLine = nh3AlarmLine;

            var update = Builders<WebsiteBulkSettings>.Update
                .Set(e => e.SiSettings, nh3);
            var filter=Builders<WebsiteBulkSettings>.Filter.Eq(e => e._id, websiteBulkSettings._id);

            await settingsCollection.UpdateOneAsync(filter, update);
            Console.WriteLine("Check Database");
        }
        
                static async Task CreateNH3BulkGasSettings() {
            IMongoClient client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("nh3_data");
            var settingsDatabase = client.GetDatabase("monitor_settings");
            var settingsCollection = settingsDatabase.GetCollection<WebsiteBulkSettings>("bulk_settings");

            var analogCollection = database.GetCollection<AnalogItem>("analog_items");
            var channel = await analogCollection
                .Find(e => e.Identifier == "Tank1 Weight")
                .FirstOrDefaultAsync();

            var websiteBulkSettings = await settingsCollection.Find(_ => true).FirstAsync();
                
            BulkGasSettings nh3 = new BulkGasSettings();
            nh3.BulkGasType = BulkGasType.H2;
            nh3.Name = channel.Identifier;
            nh3.DeviceName = "nh3_data";
            nh3.EnableAggregation = true;
            nh3.OkayLabel = "Okay";
            nh3.PointColor = KnownColor.Chartreuse;
            nh3.HoursAfter = 6;
            nh3.HoursBefore = 12;
            BulkGasAlert nh3Soft = new BulkGasAlert();
            nh3Soft.Label = ActionType.SoftWarn.ToString();
            nh3Soft.ActionType = ActionType.SoftWarn;
            nh3Soft.Default = true;
            nh3Soft.SetPoint = (int)channel.Level1SetPoint;
            
            BulkGasAlert nh3Warn = new BulkGasAlert();
            nh3Warn.Label = ActionType.Warning.ToString();
            nh3Warn.ActionType = ActionType.Warning;
            nh3Warn.Default = true;
            nh3Warn.SetPoint = (int)channel.Level2SetPoint;

            BulkGasAlert nh3Alarm = new BulkGasAlert();
            nh3Alarm.Label = ActionType.Alarm.ToString();
            nh3Alarm.ActionType = ActionType.Alarm;
            nh3Alarm.Default = true;
            nh3Alarm.SetPoint = (int)channel.Level3SetPoint;

            RefLine nh3AlarmLine = new RefLine();
            nh3AlarmLine.Value = nh3Alarm.SetPoint;
            nh3AlarmLine.Color = KnownColor.Red;
            nh3AlarmLine.Label = "Halt Production";
            
            RefLine nh3WarnLine = new RefLine();
            nh3WarnLine.Value = nh3Warn.SetPoint;
            nh3WarnLine.Color = KnownColor.Yellow;
            nh3WarnLine.Label = "Prepare To Halt";
            
            RefLine nh3SoftLine = new RefLine();
            nh3SoftLine.Value = nh3Soft.SetPoint;
            nh3SoftLine.Color = KnownColor.Wheat;
            nh3SoftLine.Label = "Notify";
            
            
            nh3.SoftAlert=nh3Soft;
            nh3.WarnAlert=nh3Warn;
            nh3.AlrmAlert=nh3Alarm;


            nh3.SoftRefLine = nh3SoftLine;
            nh3.WarnRefLine = nh3WarnLine;
            nh3.AlrmRefLine = nh3AlarmLine;
            
            var update=Builders<WebsiteBulkSettings>.Update
                .Set(e => e.NHSettings, nh3)
                .Set(e=>e.N2Settings.ChannelName,"Bulk N2(inH20)")
                .Set(e=>e.N2Settings.DeviceName,"epi1_data")
                .Set(e=>e.H2Settings.ChannelName,"Bulk H2(PSI)")
                .Set(e=>e.H2Settings.DeviceName,"epi1_data");
            var filter=Builders<WebsiteBulkSettings>.Filter.Eq(e => e._id, websiteBulkSettings._id);

            await settingsCollection.UpdateOneAsync(filter, update);
            Console.WriteLine("Check Database");
        }
        static async Task CreateBulkGasSettings() {
            IMongoClient client = new MongoClient("mongodb://172.20.3.41");
            var e1Database = client.GetDatabase("epi1_data");
            var settingsDatabase = client.GetDatabase("monitor_settings");
            var settingsCollection = settingsDatabase.GetCollection<WebsiteBulkSettings>("bulk_settings");

            var analogCollection = e1Database.GetCollection<AnalogItem>("analog_items");
            var n2Channel = await analogCollection.Find(e => e.Identifier == "Bulk N2(inH20)").FirstOrDefaultAsync();
            var h2Channel = await analogCollection.Find(e => e.Identifier == "Bulk H2(PSI)").FirstOrDefaultAsync();

            WebsiteBulkSettings webBulkSettings = new WebsiteBulkSettings();
            webBulkSettings.RefreshTime = 1800000;//30min
            BulkGasSettings h2 = new BulkGasSettings();
            h2.BulkGasType = BulkGasType.H2;
            h2.Name = h2Channel.Identifier;
            h2.EnableAggregation = true;
            h2.OkayLabel = "Okay";
            h2.PointColor = KnownColor.Chartreuse;
            h2.HoursAfter = 6;
            h2.HoursBefore = 12;
            BulkGasAlert h2soft = new BulkGasAlert();
            h2soft.Label = ActionType.SoftWarn.ToString();
            h2soft.ActionType = ActionType.SoftWarn;
            h2soft.Default = true;
            h2soft.SetPoint = (int)h2Channel.Level1SetPoint;
            
            BulkGasAlert h2warn = new BulkGasAlert();
            h2warn.Label = ActionType.Warning.ToString();
            h2warn.ActionType = ActionType.Warning;
            h2warn.Default = true;
            h2warn.SetPoint = (int)h2Channel.Level2SetPoint;

            BulkGasAlert h2alarm = new BulkGasAlert();
            h2alarm.Label = ActionType.Alarm.ToString();
            h2alarm.ActionType = ActionType.Alarm;
            h2alarm.Default = true;
            h2alarm.SetPoint = (int)h2Channel.Level3SetPoint;

            RefLine h2alarmLine = new RefLine();
            h2alarmLine.Value = h2alarm.SetPoint;
            h2alarmLine.Color = KnownColor.Red;
            h2alarmLine.Label = "Halt Production";
            
            RefLine h2warnLine = new RefLine();
            h2warnLine.Value = h2warn.SetPoint;
            h2warnLine.Color = KnownColor.Yellow;
            h2warnLine.Label = "Prepare To Halt";
            
            RefLine h2softLine = new RefLine();
            h2softLine.Value = h2soft.SetPoint;
            h2softLine.Color = KnownColor.Wheat;
            h2softLine.Label = "Notify";
            
            
            h2.SoftAlert=h2soft;
            h2.WarnAlert=h2warn;
            h2.AlrmAlert=h2alarm;


            h2.SoftRefLine = h2softLine;
            h2.WarnRefLine = h2warnLine;
            h2.AlrmRefLine = h2alarmLine;



            BulkGasSettings n2 = new BulkGasSettings();
            n2.BulkGasType = BulkGasType.N2;
            n2.Name = n2Channel.Identifier;
            n2.EnableAggregation = true;
            n2.OkayLabel = "Okay";
            n2.PointColor = KnownColor.Aqua;
            n2.HoursAfter = 6;
            n2.HoursBefore = 12;
            BulkGasAlert n2soft = new BulkGasAlert();
            n2soft.Label = ActionType.SoftWarn.ToString();
            n2soft.ActionType = ActionType.SoftWarn;
            n2soft.Default = true;
            n2soft.SetPoint = (int)n2Channel.Level1SetPoint;
            
            BulkGasAlert n2warn = new BulkGasAlert();
            n2warn.Label = ActionType.Warning.ToString();
            n2warn.ActionType = ActionType.Warning;
            n2warn.Default = true;
            n2warn.SetPoint = (int)n2Channel.Level2SetPoint;

            BulkGasAlert n2alarm = new BulkGasAlert();
            n2alarm.Label = ActionType.Alarm.ToString();
            n2alarm.ActionType = ActionType.Alarm;
            n2alarm.Default = true;
            n2alarm.SetPoint = (int)n2Channel.Level3SetPoint;

            RefLine n2alarmLine = new RefLine();
            n2alarmLine.Value = n2alarm.SetPoint;
            n2alarmLine.Color = KnownColor.Red;
            n2alarmLine.Label = "Halt Production";
            
            RefLine n2warnLine = new RefLine();
            n2warnLine.Value = n2warn.SetPoint;
            n2warnLine.Color = KnownColor.Yellow;
            n2warnLine.Label = "Prepare To Halt";
            
            RefLine n2softLine = new RefLine();
            n2softLine.Value = n2soft.SetPoint;
            n2softLine.Color = KnownColor.Wheat;
            n2softLine.Label = "Notify";

            n2.SoftAlert=n2soft;
            n2.WarnAlert=n2warn;
            n2.AlrmAlert=n2alarm;


            n2.SoftRefLine = n2softLine;
            n2.WarnRefLine = n2warnLine;
            n2.AlrmRefLine = n2alarmLine;
            
            webBulkSettings.H2Settings = h2;
            webBulkSettings.N2Settings = n2;
            await settingsCollection.InsertOneAsync(webBulkSettings);
            Console.WriteLine("Check Database");
        }

        /*static async Task TestAmmoniaController(int scale,AmmoniaController controller,ConsoleTable table) {
            var data=await controller.GetTankCalibration("172.21.100.29", scale);
            table.AddRow(data.Scale, data.ZeroRawValue,data.NonZeroRawValue,data.ZeroValue,data.NonZeroValue,
                data.GrossWeight,data.Tare,data.GasWeight,data.CurrentWeight);
            /*Console.WriteLine($"Scale: {data.Scale}");
            Console.WriteLine($"ZeroRaw: {data.ZeroRawValue}");
            Console.WriteLine($"NonZeroRaw: {data.NonZeroRawValue}");
            Console.WriteLine($"Zero: {data.ZeroValue}");
            Console.WriteLine($"NonZero: {data.NonZeroValue}");
            Console.WriteLine($"Combined: {data.Combined}");
            Console.WriteLine($"Tare: {data.Tare}");
            Console.WriteLine();
            Console.WriteLine();#1#
        }*/

        public static async Task TestExternalAlertEmail() {
            var n2Id = ObjectId.GenerateNewId();
            var h2Id = ObjectId.GenerateNewId();
            List<AlertRecord> activeAlerts = new List<AlertRecord>();
            List<AlertRecord> alerts = new List<AlertRecord>() {
                        new AlertRecord() {
                            AlertId = h2Id,
                            Bypassed = false,
                            Enabled = true,
                            CurrentState = ActionType.SoftWarn,
                            DisplayName = "Bulk H2(PSI)",
                            ChannelReading = 1000.0f,
                            Latched = false
                        },
                        new AlertRecord() {
                            AlertId = n2Id,
                            Bypassed = false,
                            Enabled = true,
                            CurrentState = ActionType.SoftWarn,
                            DisplayName = "Bulk N2(inH20)",
                            ChannelReading = 74.0f,
                            Latched = false
                        }
            };
            activeAlerts=await ProcessAlerts(alerts, activeAlerts, DateTime.Now);
            await Task.Delay(5000);

            alerts = new List<AlertRecord>() {
                new AlertRecord() {
                    AlertId = h2Id,
                    Bypassed = false,
                    Enabled = true,
                    CurrentState = ActionType.Warning,
                    DisplayName = "Bulk H2(PSI)",
                    ChannelReading = 500.0f,
                    Latched = false
                },
                new AlertRecord() {
                    AlertId = n2Id,
                    Bypassed = false,
                    Enabled = true,
                    CurrentState = ActionType.Warning,
                    DisplayName = "Bulk N2(inH20)",
                    ChannelReading = 50.0f,
                    Latched = false
                }
            };
            activeAlerts=await ProcessAlerts(alerts, activeAlerts, DateTime.Now);
            await Task.Delay(5000);
        }

        public static async Task<List<AlertRecord>> ProcessAlerts(List<AlertRecord> alerts,List<AlertRecord> activeAlerts,DateTime now) {
            bool sendEmail = false;     
            foreach (var alert in alerts) {
                if (alert.Enabled) {
                    var activeAlert = activeAlerts.FirstOrDefault(e => e.AlertId == alert.AlertId);
                    switch (alert.CurrentState) {
                        case ActionType.Okay: {
                                if (activeAlert != null) {
                                    if (!activeAlert.Latched) {
                                        activeAlert.Latched = true;
                                        activeAlert.TimeLatched = now;
                                    }
                                }
                                break;
                            }
                        case ActionType.Alarm:
                        case ActionType.Warning:
                        case ActionType.SoftWarn: {
                                if (activeAlert != null) {
                                    if (activeAlert.CurrentState != alert.CurrentState) {
                                        activeAlert.CurrentState = alert.CurrentState;
                                        activeAlert.ChannelReading = alert.ChannelReading;
                                        activeAlert.AlertAction = alert.AlertAction;
                                        activeAlert.LastAlert = now;
                                        sendEmail = true;
                                    } else {
                                        if (activeAlert.Latched) {
                                            activeAlert.Latched = false;
                                            activeAlert.TimeLatched = now;
                                        }
                                    }
                                } else {
                                    alert.LastAlert = now;
                                    activeAlerts.Add(alert.Clone());
                                    sendEmail = true;
                                }
                                break;
                            }
                        case ActionType.Maintenance:
                        case ActionType.Custom:
                        default:
                            //do nothing on maintenance,custom
                            break;
                    }
                }
            }
            if (activeAlerts.Any()) {
                if (sendEmail) {
                    var bulkH2 = activeAlerts.FirstOrDefault(e => e.DisplayName == "Bulk H2(PSI)");
                    var bulkN2 = activeAlerts.FirstOrDefault(e => e.DisplayName == "Bulk N2(inH20)");
                    await ProcessBulkGasExternalEmail(bulkN2, bulkH2,now);
                    Console.WriteLine("Email Sent");
                }
            }
            return activeAlerts;
        }
        
        private static async Task ProcessBulkGasExternalEmail(AlertRecord? bulkN2,AlertRecord? bulkH2,DateTime now) {
            if (bulkN2 != null) {
                if (bulkN2.LastAlert == now) {
                    switch (bulkN2.CurrentState) {
                        case ActionType.Warning: {
                            await SendExternalEmail("Nitrogen EMERGENCY Gas Refill Request", 
                                "Nitrogen",
                                bulkN2.ChannelReading.ToString(CultureInfo.InvariantCulture),"inH2O", 
                                "Immediately");
                            break;
                        }
                        case ActionType.SoftWarn: {
                            await SendExternalEmail("Nitrogen Gas Refill Request", 
                                "Nitrogen",
                                bulkN2.ChannelReading.ToString(CultureInfo.InvariantCulture),"inH2O", 
                                "within the next 24 Hrs");
                            break;
                        }
                        default: break;//do nothing
                    }
                }
            }
            if (bulkH2 != null) {
                if (bulkH2.LastAlert == now) {
                    switch (bulkH2.CurrentState) {
                        case ActionType.Warning: {
                            await SendExternalEmail("Hydrogen Gas EMERGENCY Refill Request", 
                                "Hydrogen",
                                bulkH2.ChannelReading.ToString(CultureInfo.InvariantCulture),"PSI", 
                                "Immediately");
                            break;
                        }
                        case ActionType.SoftWarn: {
                            await SendExternalEmail("Hydrogen Gas Refill Request", 
                                "Hydrogen",
                                bulkH2.ChannelReading.ToString(CultureInfo.InvariantCulture),"PSI", 
                                "within the next 24 Hrs");
                            break;
                        }
                        default: break;//do nothing
                    }
                }
            }
        }
        
        static async Task SendExternalEmail(string subject,string gas,string currentValue,string units,string time) {
            SmtpClient client = new SmtpClient();
            client.CheckCertificateRevocation = false;
            client.ServerCertificateValidationCallback = MyServerCertificateValidationCallback;
            await client.ConnectAsync("10.92.3.215",25,false);
            MimeMessage mailMessage = new MimeMessage();
            BodyBuilder bodyBuilder = new BodyBuilder();
            mailMessage.To.Add(new MailboxAddress("Andrew Elmendorf","aelmendorf@s-et.com"));
            mailMessage.To.Add(new MailboxAddress("Norman Culbertson","nculbertson@s-et.com"));
            mailMessage.To.Add(new MailboxAddress("Rakesh Jain","rakesh@s-et.com"));
            mailMessage.From.Add(new MailboxAddress("Monitor Alerts","monitoring@s-et.com"));
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart("plain") {
                    Text = @$"
This is an automated message notifying AirGas that Sensor Electronic Technology’s {gas} tanks need a refill {time}

Current {gas} Value: {currentValue} {units}

Please send the delivery schedule to Norman Culbertson at nculbertson@s-et.com

"
            };
            await client.SendAsync(mailMessage);
            await client.DisconnectAsync(true);
            Console.WriteLine("Check Mail");
        }

        static async Task TestModbus() {
            using var client = new TcpClient("172.20.5.24",502);
            client.ReceiveTimeout = 500;
            var modbus = ModbusIpMaster.CreateIp(client);
            var reg = await modbus.ReadHoldingRegistersAsync((byte)1,0,12);
            foreach (var value in reg) {
                Console.Write($"{(float)value/100.00}, ");
            }
            Console.WriteLine();
            Console.WriteLine("Completed");
        }
        
        static async Task WriteOutAnalogFile(string deviceName, DateTime start, DateTime stop, string fileName) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceName + "_data");

            var analogItems = database.GetCollection<AnalogItem>("analog_items").Find(_ => true).ToList();
            var analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
            Console.WriteLine("Starting query");
            var aReadings = await (await analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop)).ToListAsync();
            //var aReadings = await (await analogReadings.FindAsync(_=>true)).ToListAsync();
            var headers = analogItems.Select(e => e.Identifier).ToList();
            StringBuilder hbuilder = new StringBuilder();
            hbuilder.Append("timestamp,");
            headers.ForEach((id) => {
                hbuilder.Append(id+",");
            });
            Console.WriteLine($"Query Completed.  Count: {aReadings.Count()}");
            List<string> lines = new List<string>();
            lines.Add(hbuilder.ToString());
            foreach(var readings in aReadings) {
                StringBuilder builder = new StringBuilder();
                builder.Append(readings.timestamp.ToLocalTime().ToString()+",");
                foreach(var reading in readings.readings) {
                    builder.Append($"{reading.Value},");
                }
                lines.Add(builder.ToString());
            }
            Console.WriteLine("Writing Out Data");
            await File.WriteAllLinesAsync(fileName, lines);
            Console.WriteLine("Check File");
        }

        static async Task DtoTesting() {
            var context = new MonitorContext();
            var level = await context.AlertLevels.OfType<AnalogLevel>()
                .AsNoTracking()
                .Include(e => e.AnalogAlert)
                .Include(e => e.DeviceAction)
                .FirstOrDefaultAsync(e => e.Id.ToString() == "86B0087A-7376-4FE7-B711-08DA81EB180E");

            if (level != null) {
                var dto = level.ToDto();
                var temp = dto.ToEntity();
                context.Update(temp);
                var ret=await context.SaveChangesAsync();
                if (ret > 0) {
                    Console.WriteLine("Changes Saved, Checked Database");
                } else {
                    Console.WriteLine("Save Failed");
                }
            } else {
                Console.WriteLine("Level not found");
            }
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
            
            //bodyBuilder.HtmlBody=builder.FinishMessage();
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
                /*return cn == "Exchange2016" && issuer == "CN=Exchange2016" &&
                       serial == "3D2E6FBDF9CE1FAF46D9CC68B8D58BAB" &&
                       fingerprint == "EC14ED8D2253824E6522D19EC815AD72CC767759";*/
                return true;
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

    public class Testitem {
        public int _id { get; set; }
        public double Value { get; set; }
        
    }
}
