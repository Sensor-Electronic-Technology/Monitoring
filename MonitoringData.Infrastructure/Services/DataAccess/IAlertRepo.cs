using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MonitoringData.Infrastructure.Services.DataAccess {
    public interface IAlertRepo {
        IList<ActionItem> ActionItems { get; }
        Task<MonitorAlert> GetAlert(int alertId);
        Task LogAlerts(IEnumerable<AlertReading> alerts);
        Task UpdateAlert(int alertId, UpdateDefinition<MonitorAlert> update);
        Task Load();
    }

    public class AlertRepo : IAlertRepo {
        private IMongoCollection<ActionItem> _actionItems;
        private IMongoCollection<MonitorAlert> _monitorAlerts;
        private IMongoCollection<AlertReading> _alertReadings;
        public IList<ActionItem> ActionItems { get; private set; }
        public AlertRepo(IOptions<MonitorDatabaseSettings> databaseSettings) {
            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);
            this._actionItems = database.GetCollection<ActionItem>(databaseSettings.Value.ActionItemCollection);
            this._monitorAlerts = database.GetCollection<MonitorAlert>(databaseSettings.Value.AlertItemCollection);
            this._alertReadings = database.GetCollection<AlertReading>(databaseSettings.Value.AlertReadingCollection);
        }

        public AlertRepo(string connectionName,string databaseName,string actionColName,string alertCollName,string alertReadCol) {
            var client = new MongoClient(connectionName);
            var database = client.GetDatabase(databaseName);
            this._actionItems = database.GetCollection<ActionItem>(actionColName);
            this._monitorAlerts = database.GetCollection<MonitorAlert>(alertCollName);
            this._alertReadings = database.GetCollection<AlertReading>(alertReadCol);
        }

        public async Task LogAlerts(IEnumerable<AlertReading> alerts) {
            await this._alertReadings.InsertManyAsync(alerts);
        }

        public async Task<MonitorAlert> GetAlert(int alertId) {
            return await this._monitorAlerts.Find(e => e._id == alertId).FirstOrDefaultAsync();
        }

        public async Task UpdateAlert(int alertId,UpdateDefinition<MonitorAlert> update) {
            await this._monitorAlerts.FindOneAndUpdateAsync(e => e._id == alertId, update);
        }

        public async Task Load() {
            this.ActionItems = await this._actionItems.Find(_ => true).ToListAsync();
        }
    }
}
