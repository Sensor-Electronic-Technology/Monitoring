using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Services;

namespace MonitoringWeb.WebApp.Services; 

    public class LatestAlertService {
        private readonly ILogger<LatestAlertService> _logger;
        private readonly WebsiteConfigurationProvider _configurationProvider;
        private readonly IMongoClient _client;
        
        public LatestAlertService(ILogger<LatestAlertService> logger,
            WebsiteConfigurationProvider configurationProvider,IMongoClient client) {
            this._client = client;
            this._configurationProvider = configurationProvider;
            this._logger=logger;
        }

        public async Task<IEnumerable<LastAlertDto>> GetLatestAlarms(int days) {
            List<LastAlertDto> alertDtos = new List<LastAlertDto>();
            foreach (var device in this._configurationProvider.Devices) {
                var database = this._client.GetDatabase(device.DatabaseName);
                var alertItems = await database.GetCollection<MonitorAlert>(device.CollectionNames[nameof(MonitorAlert)])
                    .Find(e=>e.Enabled)
                    .ToListAsync();
                var alertReadings = await database.GetCollection<AlertReadings>(device.CollectionNames[nameof(AlertReadings)])
                    .Find(e=>e.timestamp>=DateTime.Now.ToLocalTime().AddDays(-days))
                    .ToListAsync();
                foreach(var alertReading in alertReadings) {
                    var alerts=alertReading.readings.Where(e => e.AlertState != ActionType.Okay && e.AlertState != ActionType.Custom);
                    foreach(var alert in alerts) {
                        var item = alertItems.FirstOrDefault(e => e._id== alert.MonitorItemId);
                        if (item != null) {
                            var temp = new LastAlertDto() {
                                channelId = item.ChannelId,
                                alertId = alert.MonitorItemId.ToString(),
                                Device = device.DeviceName,
                                database=device.DatabaseName,
                                Name =item.DisplayName,
                                State=alert.AlertState.ToString(),
                                Value=alert.Reading,
                                TimeStamp=alertReading.timestamp.ToLocalTime()
                            };
                            alertDtos.Add(temp);
                        }
                    }
                }
            }
            
            if (alertDtos.Count > 0) {
                return alertDtos;
            } else {
                return null;
            }
        }
        
        public async Task<IEnumerable<LastAlertDto>> GetLatestAlarms_Back(int days) {
            var client = new MongoClient("mongodb://172.20.3.41");
            List<LastAlertDto> alertDtos = new List<LastAlertDto>();
            var e1Database = client.GetDatabase("epi1_data");
            var e2Database = client.GetDatabase("epi2_data");
            var gasDatabase = client.GetDatabase("gasbay_data");
            var nh3Database = client.GetDatabase("nh3_data");
            

            var gasAlertItems = await gasDatabase.GetCollection<MonitorAlert>("alert_items")
                .Find(e=>e.Enabled)
                .ToListAsync();
            var gasAlerts = await gasDatabase.GetCollection<AlertReadings>("alert_readings")
                .Find(e=>e.timestamp>=DateTime.Now.ToLocalTime().AddDays(-days))
                .ToListAsync();

            foreach(var gasAlert in gasAlerts) {
                var alerts=gasAlert.readings.Where(e => e.AlertState != ActionType.Okay && e.AlertState != ActionType.Custom);
                foreach(var alert in alerts) {
                    var item = gasAlertItems.FirstOrDefault(e => e._id== alert.MonitorItemId);
                    if (item != null) {
                        var temp = new LastAlertDto() {
                            channelId = item.ChannelId,
                            alertId = alert.MonitorItemId.ToString(),
                            Device = "Gasbay",
                            database="gasbay_data_dev",
                            Name =item.DisplayName,
                            State=alert.AlertState.ToString(),
                            Value=alert.Reading,
                            TimeStamp=gasAlert.timestamp.ToLocalTime()
                        };
                        alertDtos.Add(temp);
                    }
                }
            }

            var e1AlertItems = await e1Database.GetCollection<MonitorAlert>("alert_items")
                .Find(e => e.Enabled)
                .ToListAsync();
            var e1Alerts = await e1Database.GetCollection<AlertReadings>("alert_readings")
                .Find(e => e.timestamp >= DateTime.Now.ToLocalTime().AddDays(-days))
                .ToListAsync();

            foreach (var e1Alert in e1Alerts) {
                var alerts = e1Alert.readings.Where(e => e.AlertState != ActionType.Okay && e.AlertState != ActionType.Custom);
                foreach (var alert in alerts) {
                    var item = e1AlertItems.FirstOrDefault(e => e._id == alert.MonitorItemId);
                    if (item != null) {
                        var temp = new LastAlertDto() {
                            channelId = item.ChannelId,
                            alertId = alert.MonitorItemId.ToString(),
                            Device = "Epi1",
                            database = "epi1_data_dev",
                            Name = item.DisplayName,
                            State = alert.AlertState.ToString(),
                            Value = alert.Reading,
                            TimeStamp=e1Alert.timestamp.ToLocalTime()
                        };
                        alertDtos.Add(temp);
                    }
                }
            }

            var e2AlertItems = await e2Database.GetCollection<MonitorAlert>("alert_items")
                .Find(e => e.Enabled)
                .ToListAsync();
            var e2Alerts = await e2Database.GetCollection<AlertReadings>("alert_readings")
                .Find(e => e.timestamp >= DateTime.Now.ToLocalTime().AddDays(-days))
                .ToListAsync();

            foreach (var e2Alert in e2Alerts) {
                var alerts = e2Alert.readings.Where(e => e.AlertState != ActionType.Okay && e.AlertState != ActionType.Custom);
                foreach (var alert in alerts) {
                    var item = e2AlertItems.FirstOrDefault(e => e._id == alert.MonitorItemId);
                    if (item != null) {
                        var temp = new LastAlertDto() {
                            channelId = item.ChannelId,
                            alertId = alert.MonitorItemId.ToString(),
                            Device = "Epi2",
                            database="epi2_data_dev",
                            Name = item.DisplayName,
                            State = alert.AlertState.ToString(),
                            Value = alert.Reading,
                            TimeStamp=e2Alert.timestamp.ToLocalTime()
                        };
                        alertDtos.Add(temp);
                    }
                }
            }
            
            var nh3AlertItems = await nh3Database.GetCollection<MonitorAlert>("alert_items")
                .Find(e => e.Enabled)
                .ToListAsync();
            var nh3Alerts = await nh3Database.GetCollection<AlertReadings>("alert_readings")
                .Find(e => e.timestamp >= DateTime.Now.ToLocalTime().AddDays(-days))
                .ToListAsync();

            foreach (var nh3Alert in nh3Alerts) {
                var alerts = nh3Alert.readings.Where(e => e.AlertState != ActionType.Okay && e.AlertState != ActionType.Custom);
                foreach (var alert in alerts) {
                    var item = e2AlertItems.FirstOrDefault(e => e._id == alert.MonitorItemId);
                    if (item != null) {
                        var temp = new LastAlertDto() {
                            channelId = item.ChannelId,
                            alertId = alert.MonitorItemId.ToString(),
                            Device = "NH3",
                            database="nh3_data",
                            Name = item.DisplayName,
                            State = alert.AlertState.ToString(),
                            Value = alert.Reading,
                            TimeStamp=nh3Alert.timestamp.ToLocalTime()
                        };
                        alertDtos.Add(temp);
                    }
                }
            }
            if (alertDtos.Count > 0) {
                return alertDtos;
            } else {
                return null;
            }
        }
    }