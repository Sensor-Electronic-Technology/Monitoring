using MonitoringSystem.Shared.Data.LogModel;
namespace MonitoringWeb.WebApp.Data; 

public class PlotData {
    public SensorType SensorType { get; set; }
    public IEnumerable<AnalogReadingDto> AnalogReadings { get; set; } = Enumerable.Empty<AnalogReadingDto>();
    public PlotData(SensorType sensor, IList<AnalogReadingDto> readings) {
        this.SensorType = sensor;
        this.AnalogReadings=readings;
    }
    public PlotData() {
        
    }
}