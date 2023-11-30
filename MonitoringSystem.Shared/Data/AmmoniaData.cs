using MonitoringSystem.Shared.Data.LogModel;
namespace MonitoringSystem.Shared.Data; 

public class AmmoniaData {
    public int Scale { get; set; }
    public int ZeroRawValue { get; set; }
    public int NonZeroRawValue { get; set; }
    public int ZeroValue { get; set; }
    public int NonZeroValue { get; set; }
    public int GrossWeight { get; set; }
    public int GasWeight { get; set; }
    public int Tare { get; set; }
    public int CurrentWeight { get; set; }

    public AmmoniaData() {
        
    }

    public void ClearTankWeight(int scale, Calibration logCal) {
        this.Scale = scale;
        this.ZeroValue = logCal.ZeroValue;
        this.ZeroRawValue = logCal.ZeroRawValue;
        this.NonZeroValue = logCal.NonZeroValue;
        this.NonZeroRawValue = logCal.NonZeroRawValue;
        this.GrossWeight = 0;
        this.Tare = 0;
        this.GasWeight = 0;
        this.CurrentWeight = 0;
    }

    public void SetTankWeight(int scale, Calibration scaleCalibration, TankWeight tankWeight) {
        this.Scale = scale;
        this.ZeroValue = scaleCalibration.ZeroValue;
        this.ZeroRawValue = scaleCalibration.ZeroRawValue;
        this.NonZeroValue = scaleCalibration.NonZeroValue;
        this.NonZeroRawValue = scaleCalibration.NonZeroRawValue;
        this.GasWeight = tankWeight.Gas;
        this.GrossWeight = tankWeight.Gross;
        this.Tare = tankWeight.Tare;
    }

    public AmmoniaData(int scale,Calibration logCal) {
        this.Scale = scale;
        this.ZeroValue = logCal.ZeroValue;
        this.ZeroRawValue = logCal.ZeroRawValue;
        this.NonZeroValue = logCal.NonZeroValue;
        this.NonZeroRawValue = logCal.NonZeroRawValue;
        this.GrossWeight = 0;
        this.GasWeight = 0;
        this.Tare = 0;
        this.CurrentWeight = 0;
    }

    public AmmoniaData(int scale, Calibration logCal, TankWeight tankWeight):this(scale,logCal) {
        this.GrossWeight = tankWeight.Gross;
        this.GasWeight = tankWeight.Gas;
        this.Tare = tankWeight.Tare;
        this.CurrentWeight = 0;
    }
}