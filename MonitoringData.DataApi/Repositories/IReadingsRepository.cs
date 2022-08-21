namespace MonitoringData.DataApi.Repositories; 

public interface IReadingsRepository<T> {
    Task InsertReadings(T readings);
}