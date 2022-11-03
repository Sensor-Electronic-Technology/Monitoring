namespace MonitoringSystem.Shared.Data.EntityDtos;

public class SensorDto {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public double RecordThreshold { get; set; }
    public int ThresholdInterval { get; set; }
    public ValueDirection ValueDirection { get; set; }
    public double Slope { get; set; }
    public double Offset { get; set; }
    public double Factor { get; set; }
    public string? Units { get; set; }
    public int YAxisMin { get; set; }
    public int YAxisMax { get; set; }
}