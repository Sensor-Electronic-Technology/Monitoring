namespace MonitoringWeb.WebApp.Data;

public class H2AlertCalcDto {
    public double LastReading { get; set; }
    public double Rate { get; set; }
    public Estimates Estimates { get; set; }
    public int Days { get; set; }
}