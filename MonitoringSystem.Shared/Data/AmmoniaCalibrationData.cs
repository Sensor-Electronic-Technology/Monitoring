namespace MonitoringSystem.Shared.Data; 

public class AmmoniaCalibrationData {
    public int CalZero { get; set; }
    public int CalNonZero { get; set; }
    public int ActualZero { get; set; }
    public int ActualNonZero { get; set; }
    public int TotalWeight { get; set; }
    public int GasWeight { get; set; }
    public ushort Tank { get; set; }
}