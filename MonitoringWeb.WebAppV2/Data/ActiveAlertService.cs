using MonitoringSystem.Shared.Data;

namespace MonitoringWeb.WebAppV2.Data; 

public class ActiveAlertService {
    public EventHandler? ActiveAlertsChanged { get; set; }
    
    private object syncLock=new object();

    public List<AlertDto> ActiveAlerts {
        get => this._activeAlerts;
    }

    private List<AlertDto> _activeAlerts;

    public ActiveAlertService() {
        this._activeAlerts = new List<AlertDto>();
    }
    
    public void AddActiveAlert(AlertDto alert) {
        lock (syncLock) {
            if (this._activeAlerts.FirstOrDefault(e => e.alertId == alert.alertId) != null) {
                this._activeAlerts.Add(alert);
                this.ActiveAlertsChanged?.Invoke(null,EventArgs.Empty);   
            }
        }
    }

    public void ClearActiveAlert(AlertDto alert) {
        lock (syncLock) {
            this._activeAlerts.RemoveAll(e => e.alertId == alert.alertId);
            this.ActiveAlertsChanged?.Invoke(null,EventArgs.Empty);
        }
    }
}