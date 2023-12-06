namespace MonitoringWeb.WebApp.Data; 

public class Nh3TankFormSettings {
    public string MeasuredWeightHeader { get; set; } = "Measured Weights";
    public string LabeledWeightHeader { get; set; } = "Labeled Weights";
    public bool CalModeActive { get; set; } = false;
    public bool CalibrationDone { get; set; }
    public bool LabeledWeightsDone { get; set; } = false;
    
    
}