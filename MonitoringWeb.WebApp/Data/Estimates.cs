namespace MonitoringWeb.WebApp.Data;

public class Estimates {
    public DateTime SoftWarnDate { get; set; }
    public DateTime WarningDate { get; set; }
    public DateTime AlarmDate { get; set; }

    public double SoftWarn { get; set; }
    public double Warning { get; set; }
    public double Alarm { get; set; }
}