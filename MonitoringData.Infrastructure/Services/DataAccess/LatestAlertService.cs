﻿using System.Text;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;

public class AlertDto {
    public int alertId { get; set; }
    public int channelId { get; set; }
    public AlertItemType ItemType { get; set; }
    public string Name { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Device { get; set; }
    public string database { get; set; }
    public string State { get; set; }
    public float Value { get; set; }
}



namespace MonitoringData.Infrastructure.Services.DataAccess {
    public class LatestAlertService {
        private readonly ILogger<LatestAlertService> _logger;
        private readonly MonitorDatabaseSettings _databaseSettings;


        public async Task<IEnumerable<AlertDto>> GetLatestAlarms(int days) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var e1Database = client.GetDatabase("epi1_data");
            var e2Database = client.GetDatabase("epi2_data");
            var gasDatabase = client.GetDatabase("gasbay_data");
            List<AlertDto> alertDtos = new List<AlertDto>();

            var gasAlertItems = await gasDatabase.GetCollection<MonitorAlert>("alert_items")
                .Find(e=>e.enabled)
                .ToListAsync();
            var gasAlerts = await gasDatabase.GetCollection<AlertReadings>("alert_readings")
                .Find(e=>e.timestamp>=DateTime.Now.ToLocalTime().AddDays(-days))
                .ToListAsync();

            foreach(var gasAlert in gasAlerts) {
                var alerts=gasAlert.readings.Where(e => e.state != ActionType.Okay && e.state != ActionType.Custom);
                foreach(var alert in alerts) {
                    var item = gasAlertItems.FirstOrDefault(e => e._id == alert.itemid);
                    if (item != null) {
                        var temp = new AlertDto() {
                            channelId = item.channelId,
                            alertId = alert.itemid,
                            Device = "Gasbay",
                            database="gasbay_data",
                            Name =item.displayName,
                            State=alert.state.ToString(),
                            Value=alert.reading,
                            TimeStamp=gasAlert.timestamp.ToLocalTime()
                        };
                        alertDtos.Add(temp);
                    }
                }
            }

            var e1AlertItems = await e1Database.GetCollection<MonitorAlert>("alert_items")
                .Find(e => e.enabled)
                .ToListAsync();
            var e1Alerts = await e1Database.GetCollection<AlertReadings>("alert_readings")
                .Find(e => e.timestamp >= DateTime.Now.ToLocalTime().AddDays(-days))
                .ToListAsync();

            foreach (var e1Alert in e1Alerts) {
                var alerts = e1Alert.readings.Where(e => e.state != ActionType.Okay && e.state != ActionType.Custom);
                foreach (var alert in alerts) {
                    var item = e1AlertItems.FirstOrDefault(e => e._id == alert.itemid);
                    if (item != null) {
                        var temp = new AlertDto() {
                            channelId = item.channelId,
                            alertId = alert.itemid,
                            Device = "Epi1",
                            database = "epi1_data",
                            Name = item.displayName,
                            State = alert.state.ToString(),
                            Value = alert.reading,
                            TimeStamp=e1Alert.timestamp.ToLocalTime()
                        };
                        alertDtos.Add(temp);
                    }
                }
            }

            var e2AlertItems = await e2Database.GetCollection<MonitorAlert>("alert_items")
                .Find(e => e.enabled)
                .ToListAsync();
            var e2Alerts = await e2Database.GetCollection<AlertReadings>("alert_readings")
                .Find(e => e.timestamp >= DateTime.Now.ToLocalTime().AddDays(-days))
                .ToListAsync();

            foreach (var e2Alert in e2Alerts) {
                var alerts = e2Alert.readings.Where(e => e.state != ActionType.Okay && e.state != ActionType.Custom);
                foreach (var alert in alerts) {
                    var item = e2AlertItems.FirstOrDefault(e => e._id == alert.itemid);
                    if (item != null) {
                        var temp = new AlertDto() {
                            channelId = item.channelId,
                            alertId = alert.itemid,
                            Device = "Epi2",
                            database="epi2_data",
                            Name = item.displayName,
                            State = alert.state.ToString(),
                            Value = alert.reading,
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
}