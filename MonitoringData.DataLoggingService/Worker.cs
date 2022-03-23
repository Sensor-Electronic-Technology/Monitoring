using MonitoringData.Infrastructure.Services;

namespace MonitoringData.DataLoggingService {
    public class Worker : IHostedService, IDisposable {
        private readonly ILogger<Worker> _logger;
        private readonly IDataLogger _dataLogger;

        private Timer _timer;

        public Worker(ILogger<Worker> logger,IDataLogger dataLogger) {
            _logger = logger;
            this._dataLogger = dataLogger;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            await this._dataLogger.Load();
            this._timer = new Timer(this.DataLogHandler, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public async void DataLogHandler(object state) {
            await this._dataLogger.Read();
            this._logger.LogInformation("Logged Data");
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            this._logger.LogInformation("Logger Stopped");
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }
}