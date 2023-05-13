using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MonitoringData.Infrastructure.Events;
using MonitoringData.Infrastructure.Services;
using MonitoringData.Infrastructure.Services.DataLogging;

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
            //await DownloadImage();
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
        
        private async Task DownloadImage() {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("monitor_settings");
            this._logger.LogInformation("Starting download..");
            var bucket = new GridFSBucket(database, new GridFSBucketOptions {
                BucketName = "website_images"
            });
            var options = new GridFSDownloadByNameOptions { Revision = -1 };
            using (var stream = new FileStream(@"GasDetectorMap_New.png", FileMode.Append,FileAccess.Write)) {
                await bucket.DownloadToStreamByNameAsync("GasDetectorMap.png", stream);
            }
            
            if (File.Exists("GasDetectorMap_New.png")) {
                if (File.Exists("GasDetectorMap.png")) {
                    File.Delete("GasDetectorMap.png");
                }
                File.Copy("GasDetectorMap_New.png", "GasDetectorMap.png");
                File.SetAttributes("GasDetectorMap.png",FileAttributes.Normal);
                File.Delete("GasDetectorMap_New.png");
                this._logger.LogInformation("New floor plan downloaded");
            } else {
                if (!File.Exists("GasDetectorMap.png")) {
                    this._logger.LogCritical("Download failed and no existing floor plan found");
                } else {
                    this._logger.LogWarning("GasDetectorMap.png not replaced.  Download Failed");
                }
            }
        }
    }
}