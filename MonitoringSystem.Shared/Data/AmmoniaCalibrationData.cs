namespace MonitoringSystem.Shared.Data; 

public class AmmoniaCalibrationData {
    public int Scale { get; set; }
    public int ZeroRawValue { get; set; }
    public int NonZeroRawValue { get; set; }
    public int ZeroValue { get; set; }
    public int NonZeroValue { get; set; }
    public int Combined { get; set; }
    public int GasWeight { get; set; }
    public int Tare { get; set; }
    public int CurrentWeight { get; set; }
}