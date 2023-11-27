using MongoDB.Bson;
namespace MonitoringSystem.Shared.Data.LogModel;

public enum TankState {
    InUse,
    IdleOnScaleMeasured,
    IdleOnScaleNotMeasured,
    Consumed,
}


public class TankScale {
    public ObjectId _id { get; set; }
    public int ScaleId { get; set; }
    public bool HasTank { get; set; }
    public NH3Tank? CurrentTank { get; set; }
    public Calibration CurrentCalibration { get; set; }
    public List<Calibration> CalibrationLog { get; set; } = new List<Calibration>();
    public List<NH3Tank> Nh3TankLog { get; set; } = new List<NH3Tank>();
}

public class NH3Tank {
    public string? SerialNumber { get; set; }
    public TankState TankState { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime StopDate { get; set; }
    private bool Measured { get; set; }
    public double StartWeight { get; set; }
    public double StopWeight { get; set; }
    public double Consumption { get; set; }
    public TankWeight? LabeledWeight { get; set; }
    public TankWeight? MeasuredWeight { get; set; }
    
    
}

public class Calibration {
    public DateTime CalibrationDate { get; set; }
    public bool IsCurrent { get; set; }
    public int ZeroRawValue { get; set; }
    public int NonZeroRawValue { get; set; }
    public int ZeroValue { get; set; }
    public int NonZeroValue { get; set; }
    
}

public class TankWeight {
    public double Gross { get; set; }
    public double Tare { get; set; }
    public double Gas { get; set; }
    public double Frame { get; set; }
}