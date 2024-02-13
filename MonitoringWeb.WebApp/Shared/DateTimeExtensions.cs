namespace MonitoringWeb.WebApp.Shared; 

public static class DateTimeExtensions {
    public static string DateTimeLocal(this DateTime dt) {
        if (dt.IsDaylightSavingTime()) {
            return dt.AddHours(-5).ToString("MM/dd/yy hh:mm:ss tt");
        } else {
            return dt.AddHours(-4).ToString("MM/dd/yy hh:mm:ss tt");
        }
    }
}