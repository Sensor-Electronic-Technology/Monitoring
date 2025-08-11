namespace MonitoringData.Infrastructure.Services.DataLogging;

public class DeviceCheck {
    private static int FailLimit=20;
    private bool _offlineLatch;
    private int _failCount;
    private DateTime _offlineTime;
        
    public void Clear() {
        this._offlineLatch = false;
        this._failCount = 0;
    }
        
    public bool CheckTime(DateTime now) {
        if (this._failCount >= DeviceCheck.FailLimit) {
            if (!this._offlineLatch) {
                this._offlineLatch = true;
                this._offlineTime = now;
                return true;
            } else {
                if ((now - this._offlineTime).TotalMinutes >= 30) {
                    this._offlineTime = now;
                    return true;
                }
                return false;
            }
        } else {
            this._failCount++;
            return false;
        }
    }
}