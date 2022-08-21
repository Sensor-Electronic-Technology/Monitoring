using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit.Futures.Contracts;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;


namespace MonitoringData.Infrastructure.Services.DataAccess {
    public interface IAlertRepo {
        IList<ActionItem> ActionItems { get; }
        ManagedDevice ManagedDevice { get; }
        Task LogAlerts(AlertReadings alerts);
        Task Load();
        Task Reload();
    }

    public class AlertRepo : IAlertRepo {
        private IMongoCollection<ActionItem> _actionItems;
        private IMongoCollection<MonitorAlert> _monitorAlerts;
        private IMongoCollection<AlertReadings> _alertReadings;
        private readonly DataLogConfigProvider _configProvider;
        private ManagedDevice _device;
        private readonly IMongoClient _client;
        public IList<ActionItem> ActionItems { get; private set; }
        public ManagedDevice ManagedDevice => this._device;

        public AlertRepo(IMongoClient client,DataLogConfigProvider configProvider) {
            this._client = client;
            this._configProvider = configProvider;
            this._device = configProvider.ManagedDevice;
            var database = client.GetDatabase(this._device.DatabaseName);
            this._actionItems = database.GetCollection<ActionItem>(this._device.CollectionNames[nameof(ActionItem)]);
            this._monitorAlerts = database.GetCollection<MonitorAlert>(this._device.CollectionNames[nameof(MonitorAlert)]);
            this._alertReadings = database.GetCollection<AlertReadings>(this._device.CollectionNames[nameof(AlertReadings)]);
        }
        
        public async Task LogAlerts(AlertReadings alerts) {
            await this._alertReadings.InsertOneAsync(alerts);
        }

        public async Task Load() {
            this.ActionItems = await this._actionItems.Find(_ => true).ToListAsync();
        }

        public async Task Reload() {
            this._device = this._configProvider.ManagedDevice;
            var database = this._client.GetDatabase(this._device.DatabaseName);
            this._actionItems = database.GetCollection<ActionItem>(this._device.CollectionNames[nameof(ActionItem)]);
            this._monitorAlerts = database.GetCollection<MonitorAlert>(this._device.CollectionNames[nameof(MonitorAlert)]);
            this._alertReadings = database.GetCollection<AlertReadings>(this._device.CollectionNames[nameof(AlertReadings)]);
            this.ActionItems = await this._actionItems.Find(_ => true).ToListAsync();
        }
    }
}
