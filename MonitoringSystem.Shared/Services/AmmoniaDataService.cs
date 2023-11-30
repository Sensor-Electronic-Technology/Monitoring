using MongoDB.Driver;
using MonitoringSystem.Shared.Data.LogModel;
namespace MonitoringSystem.Shared.Services; 

public class AmmoniaDataService {
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<TankScale> _tankScaleCollection;

    public AmmoniaDataService(IMongoClient client) {
        this._database = client.GetDatabase("nh3_logs");
        this._tankScaleCollection = this._database.GetCollection<TankScale>("tank_scales");
    }

    public async Task<List<TankScale>> GetTankScales() {
        return await this._tankScaleCollection.Find(_ => true).ToListAsync();
    }

    public async Task<TankScale?> GetTankScale(int scale) {
        return await this._tankScaleCollection.Find(e => e.ScaleId == scale).FirstOrDefaultAsync();
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
            var updateResult=await this._tankScaleCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }
        return false;
    }
}