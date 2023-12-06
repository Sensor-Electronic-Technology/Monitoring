using MongoDB.Bson;
namespace MonitoringSystem.Shared.Data.LogModel;

public enum TankScaleState {
    InUse,
    IdleOnScaleMeasured,
    IdleOnScaleNotMeasured,
    Consumed,
    NoTank
}


public class TankScale {
    public ObjectId _id { get; set; }
    public int ScaleId { get; set; }
    public string ChannelName { get; set; }
    public TankScaleState TankScaleState { get; set; }
    public NH3Tank? CurrentTank { get; set; }
    public Calibration CurrentCalibration { get; set; }
    public List<Calibration> CalibrationLog { get; set; } = new List<Calibration>();
    public List<NH3Tank> Nh3TankLog { get; set; } = new List<NH3Tank>();
}

public class NH3Tank {
    public string? SerialNumber { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime StopDate { get; set; }
    public int StartWeight { get; set; }
    public int StopWeight { get; set; }
    public double ConsumptionPerHr { get; set; }
    public double ConsumptionPerDay { get; set; }
    public TankWeight? LabeledWeight { get; set; } = new TankWeight();
    public TankWeight? MeasuredWeight { get; set; } = new TankWeight();

    public NH3Tank() {
        
    }
}

public class WeightReading {
    public ObjectId _id { get; set; }
    public DateTime timestamp { get; set; }
    public string? ChannelName { get; set; }
    public double Value { get; set; }
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
    public int Gross { get; set; }
    public int Tare { get; set; }
    public int Gas { get; set; }
}