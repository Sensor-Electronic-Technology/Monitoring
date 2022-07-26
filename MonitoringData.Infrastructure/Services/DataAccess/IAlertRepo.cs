﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit.Futures.Contracts;


namespace MonitoringData.Infrastructure.Services.DataAccess {
    public interface IAlertRepo {
        IList<ActionItem> ActionItems { get; }
        ManagedDevice ManagedDevice { get; }
        Task<MonitorAlert> GetAlert(int alertId);
        Task LogAlerts(AlertReadings alerts);
        Task UpdateAlert(int alertId, UpdateDefinition<MonitorAlert> update);
        Task Load();
    }

    public class AlertRepo : IAlertRepo {
        private IMongoCollection<ActionItem> _actionItems;
        private IMongoCollection<MonitorAlert> _monitorAlerts;
        private IMongoCollection<AlertReadings> _alertReadings;
        private readonly MonitorDatabaseSettings _settings;
        private readonly DataLogConfigProvider _configProvider;
        private readonly ManagedDevice _device;
        public IList<ActionItem> ActionItems { get; private set; }
        public ManagedDevice ManagedDevice => this._device;

        public AlertRepo(IMongoClient client,DataLogConfigProvider configProvider) {
            this._configProvider = configProvider;
            this._device = configProvider.ManagedDevice;
            var database = client.GetDatabase(this._device.DatabaseName);
            this._actionItems = database.GetCollection<ActionItem>(this._device.CollectionNames[nameof(ActionItem)]);
            this._monitorAlerts = database.GetCollection<MonitorAlert>(this._device.CollectionNames[nameof(MonitorAlert)]);
            this._alertReadings = database.GetCollection<AlertReadings>(this._device.CollectionNames[nameof(AlertReadings)]);
        }

        public AlertRepo(string connectionName,string databaseName,string actionColName,string alertCollName,string alertReadCol) {
            var client = new MongoClient(connectionName);
            var database = client.GetDatabase(databaseName);
            this._actionItems = database.GetCollection<ActionItem>(actionColName);
            this._monitorAlerts = database.GetCollection<MonitorAlert>(alertCollName);
            this._alertReadings = database.GetCollection<AlertReadings>(alertReadCol);
        }

        public async Task LogAlerts(AlertReadings alerts) {
            await this._alertReadings.InsertOneAsync(alerts);
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
