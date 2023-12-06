using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.LogModel;
namespace MonitoringSystem.Shared.Services; 

public class AmmoniaDataService {
    private readonly IMongoCollection<TankScale> _tankScaleCollection;
    private readonly IMongoCollection<WebsiteBulkSettings> _bulkSettingsCollection;

    public AmmoniaDataService(IMongoClient client,IOptions<MonitorWebsiteSettings> options) {
        var tankScaleDatabase= client.GetDatabase(options.Value.NH3LogDatabase);
        this._tankScaleCollection = tankScaleDatabase.GetCollection<TankScale>(options.Value.TankScaleCollection);
        var bulkDatabase = client.GetDatabase(options.Value.DatabaseName);
        this._bulkSettingsCollection = bulkDatabase.GetCollection<WebsiteBulkSettings>(options.Value.BulkSettingsCollection);
    }

    public async Task<List<TankScale>> GetTankScales() {
        return await this._tankScaleCollection.Find(_ => true).ToListAsync();
    }

    public async Task<TankScale?> GetTankScale(int scale) {
        return await this._tankScaleCollection.Find(e => e.ScaleId == scale).FirstOrDefaultAsync();
    }

    public async Task SwitchTankToConsuming(TankScale scale) {
        var settings=await this._bulkSettingsCollection.Find(_ => true).FirstOrDefaultAsync();
        if (settings != null) {
            var tankScaleNew = await this.GetTankScale(scale.ScaleId);
            var tankScaleOld = await this._tankScaleCollection
                .Find(e => e.ChannelName == settings.NHSettings.ChannelName)
                .FirstOrDefaultAsync();
            if (tankScaleNew?.CurrentTank != null && tankScaleOld?.CurrentTank!=null) {
                var now = DateTime.Now;
                var oldTank = tankScaleOld.CurrentTank;
                var newTank = tankScaleNew.CurrentTank;
                var nh3Settings = settings.NHSettings;
                newTank.StartDate = now;
                oldTank.StopDate = now;
                nh3Settings.ChannelName = tankScaleNew.ChannelName;
                nh3Settings.Name = tankScaleNew.ChannelName;
                
                var oldTankFilter = Builders<TankScale>.Filter
                    .Eq(e => e._id, tankScaleOld._id);
                
                var oldTankUpdate = Builders<TankScale>.Update
                    .Set(e => e.TankScaleState, TankScaleState.Consumed)
                    .Set(e=>e.CurrentTank,oldTank);

                var newTankFilter = Builders<TankScale>.Filter
                    .Eq(e => e._id, tankScaleNew._id);
                
                var newTankUpdate = Builders<TankScale>.Update
                    .Set(e => e.TankScaleState, TankScaleState.InUse)
                    .Set(e=>e.CurrentTank,newTank);
        
                var settingsUpdate=Builders<WebsiteBulkSettings>.Update
                    .Set(e => e.NHSettings, nh3Settings);
                
                var settingsFilter = Builders<WebsiteBulkSettings>.Filter.Eq(e => e._id, settings._id);

                await this._tankScaleCollection.UpdateOneAsync(oldTankFilter, oldTankUpdate);
                await this._tankScaleCollection.UpdateOneAsync(newTankFilter, newTankUpdate);
                await this._bulkSettingsCollection.UpdateOneAsync(settingsFilter, settingsUpdate);
            }
        }
    }

    public async Task<bool> RemoveCurrentTank(int scale,int lastWeight) {
        var tankScale = await this._tankScaleCollection
            .Find(e => e.ScaleId == scale)
            .FirstOrDefaultAsync();
        if (tankScale != null) {
            if (tankScale.CurrentTank != null) {
                var tank = tankScale.CurrentTank;
                tank.StopWeight = lastWeight;
                int dWeight = (tank.StartWeight - tank.StopWeight);
                int dt = (tank.StopDate - tank.StartDate).Hours;
                tank.ConsumptionPerHr = (double)dWeight/ dt;
                tank.ConsumptionPerDay = tank.ConsumptionPerHr / 24;
                var filter = Builders<TankScale>.Filter.Eq(e=>e.ScaleId,scale);
                var update = Builders<TankScale>.Update
                    .Set(e => e.CurrentTank, null)
                    .Set(e=>e.TankScaleState,TankScaleState.NoTank)
                    .Push(e => e.Nh3TankLog, tank);
                var response=await this._tankScaleCollection.UpdateOneAsync(filter, update);
                if (response.IsAcknowledged) {
                    if (response.ModifiedCount > 0) {
                        return true;
                    }
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public async Task<bool> AddCurrentTank(int scale, NH3Tank tank) {
        var tankScale = await this._tankScaleCollection.Find(e => e.ScaleId == scale).FirstOrDefaultAsync();
        if (tankScale != null) {
            var update = Builders<TankScale>.Update
                .Set(e=>e.TankScaleState,TankScaleState.IdleOnScaleMeasured)
                .Set(e => e.CurrentTank, tank);
            var filter = Builders<TankScale>.Filter.Eq(e => e.ScaleId, scale);
            var response = await this._tankScaleCollection.UpdateOneAsync(filter, update);
            if (response.IsAcknowledged) {
                if (response.ModifiedCount > 0) {
                    return true;
                }
                return false;
            }
            return false;
        } else {
            return false;
        }
    }

    public async Task<bool> AddNewCurrentCalibration(TankScale scale,Calibration calibration) {
        var tankScale=await this.GetTankScale(scale.ScaleId);
        if (tankScale != null) {
            var calibrations = tankScale.CalibrationLog;
            calibrations.ForEach(cal => {
                cal.IsCurrent = false;
            });
            calibration.IsCurrent = true;
            calibrations.Add(calibration);
            var update = Builders<TankScale>.Update
                .Set(e => e.CalibrationLog, calibrations)
                .Set(e=>e.CurrentCalibration,calibration);
            var filter = Builders<TankScale>.Filter.Eq(e => e._id,scale._id);
            var response=await this._tankScaleCollection.UpdateOneAsync(filter, update);
            if (response.IsAcknowledged) {
                if (response.ModifiedCount > 0) {
                    return true;
                }
                return false;
            }
        }
        return false;
    }
}