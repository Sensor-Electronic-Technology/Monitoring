namespace MonitoringData.Infrastructure.Services.DataLogging {
    public interface IDataLogger {
        Task Read();
        Task Load();
        Task Reload();
    }
}
