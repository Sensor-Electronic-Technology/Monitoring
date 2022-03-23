using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringData.DataLoggingService;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services;
using MonitoringData.Infrastructure.Services.DataAccess;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostContext, configuration) => {
    configuration.AddJsonFile(MonitorDatabaseSettings.FileName, optional: true, reloadOnChange: true);
});

builder.ConfigureServices((hostContext, services) => {
    services.Configure<MonitorDatabaseSettings>(hostContext.Configuration.GetSection(MonitorDatabaseSettings.SectionName));
    services.AddSingleton<IMonitorDataRepo,MonitorDataService>();
    services.AddDbContext<FacilityContext>();
    services.AddSingleton<IAlertRepo,AlertRepo>();
    services.AddTransient<IDataLogger,ModbusDataLogger>();
    services.AddHostedService<Worker>();
});

IHost host = builder.Build();

await host.RunAsync();
