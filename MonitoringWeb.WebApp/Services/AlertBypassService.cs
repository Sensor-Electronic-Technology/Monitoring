using MongoDB.Driver;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringWeb.WebApp.Data;

namespace MonitoringWeb.WebApp.Services;

public class AlertBypassService {
    private readonly IMongoCollection<BypassAlert> _epi1AlertCollection;
    private readonly IMongoCollection<BypassAlert> _epi2AlertCollection;
    private readonly IMongoCollection<BypassAlert> _gasBayAlertCollection;
    
    public AlertBypassService(IMongoClient client) {
        this._epi1AlertCollection = client.GetDatabase("epi1_data").GetCollection<BypassAlert>("bypass_alerts");
        this._epi2AlertCollection = client.GetDatabase("epi2_data").GetCollection<BypassAlert>("bypass_alerts");
        this._gasBayAlertCollection = client.GetDatabase("gasbay_data").GetCollection<BypassAlert>("bypass_alerts");
    }

    public Task<List<BypassAlert>> GetEpi1MonitorAlerts() {
        return this._epi1AlertCollection.Find(e=>e.Enabled).ToListAsync();
    }
    
    public Task<List<BypassAlert>> GetEpi2MonitorAlerts() {
        return this._epi2AlertCollection.Find(e=>e.Enabled).ToListAsync();
    }
    
    public Task<List<BypassAlert>> GetGasBayMonitorAlerts() {
        return this._gasBayAlertCollection.Find(e=>e.Enabled).ToListAsync();
    }

    public async Task<bool> UpdateEpi1State(BypassAlert alert) {
        var update = Builders<BypassAlert>.Update
            .Set(e => e.Bypassed, alert.Bypassed)
            .Set(e => e.TimeBypassed, alert.Bypassed ? DateTime.UtcNow : DateTime.MaxValue);
        
        var updateResult=await this._epi1AlertCollection.UpdateOneAsync(e => e._id == alert._id, update);
        return updateResult.IsAcknowledged;
    }
    
    public async Task<bool> UpdateEpi1ResetTime(BypassAlert alert) {
        var update = Builders<BypassAlert>.Update
            .Set(e => e.Bypassed, alert.Bypassed);
        if(alert.Bypassed)
            update.Set(e=>e.TimeBypassed,DateTime.UtcNow);
        
        var updateResult=await this._epi1AlertCollection.UpdateOneAsync(e => e._id == alert._id, update);
        return updateResult.IsAcknowledged;
    }
    
    /*public async Task<bool> UpdateEpi2BypassAlert(BypassAlert alert) {
        var update = Builders<MonitorAlert>.Update
            .Set(e => e.Enabled, alert.State)
            .Set(e => e.BypassResetTime, alert.BypassTimer);
        var updateResult=await this._epi2AlertCollection.UpdateOneAsync(e => e._id == alert.AlertId, update);
        return updateResult.IsAcknowledged;
    }
    
    public async Task<bool> UpdateGasBayBypassAlert(BypassAlert alert) {
        var update = Builders<MonitorAlert>.Update
            .Set(e => e.Enabled, alert.State)
            .Set(e => e.BypassResetTime, alert.BypassTimer);
        var updateResult=await this._gasBayAlertCollection.UpdateOneAsync(e => e._id == alert.AlertId, update);
        return updateResult.IsAcknowledged;
    }*/
    
}