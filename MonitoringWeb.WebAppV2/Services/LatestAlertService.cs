using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.LogModel;
namespace MonitoringWeb.WebAppV2.Services; 

    public class LatestAlertService {
        private readonly ILogger<LatestAlertService> _logger;
        private readonly WebsiteConfigurationProvider _configurationProvider;
        
        public LatestAlertService(ILogger<LatestAlertService> logger,
            WebsiteConfigurationProvider configurationProvider) {
            this._configurationProvider = configurationProvider;
            this._logger=logger;
        }

        public async Task<IEnumerable<LastAlertDto>> GetLatestAlarms(int days) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var e1Database = client.GetDatabase("epi1_data");
            var e2Database = client.GetDatabase("epi2_data");
            var gasDatabase = client.GetDatabase("gasbay_data");
            List<LastAlertDto> alertDtos = new List<LastAlertDto>();

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
                            channelId = item.MonitorBoxItemId.ToString(),
                            alertId = alert.MonitorItemId.ToString(),
                            Device = "Gasbay",
                            database="gasbay_data",
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
                            channelId = item.MonitorBoxItemId.ToString(),
                            alertId = alert.MonitorItemId.ToString(),
                            Device = "Epi1",
                            database = "epi1_data",
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
                            channelId = item.MonitorBoxItemId.ToString(),
                            alertId = alert.MonitorItemId.ToString(),
                            Device = "Epi2",
                            database="epi2_data",
                            Name = item.DisplayName,
                            State = alert.AlertState.ToString(),
                            Value = alert.Reading,
                            TimeStamp=e2Alert.timestamp.ToLocalTime()
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