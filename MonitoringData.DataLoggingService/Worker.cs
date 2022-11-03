using MassTransit;
using MonitoringData.Infrastructure.Events;
using MonitoringData.Infrastructure.Services;

namespace MonitoringData.DataLoggingService {
    public class Worker : IHostedService, IDisposable,IConsumer<ReloadConsumer>{
        private readonly ILogger<Worker> _logger;
        private readonly IDataLogger _dataLogger;
        private System.Timers.Timer _timer;
        //private Timer _timer;

        public Worker(ILogger<Worker> logger,IDataLogger dataLogger) {
            _logger = logger;
            this._dataLogger = dataLogger;
            this._timer = new(interval: 1000);
            this._timer.Elapsed += async (sender, e) => {
                await this.DataLogHandler();
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            await this._dataLogger.Load();
            this._timer.Start();
        }

        private async Task DataLogHandler() {
            await this._dataLogger.Read();
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            this._timer.Stop();
            return Task.CompletedTask;
        }

        public void Dispose() {
            if (this._timer != null) {
                this._timer.Dispose();
            }
        }
        
        public async Task Consume(ConsumeContext<ReloadConsumer> context) {
            CancellationTokenSource source = new CancellationTokenSource();
            this._logger.LogInformation("Stopping Service");
            await this.StopAsync(source.Token);
            this._logger.LogInformation("Reloading Configuration");
            await this._dataLogger.Reload();
            this._logger.LogInformation("Starting Service");
            await this.StartAsync(source.Token);
        }
    }
}