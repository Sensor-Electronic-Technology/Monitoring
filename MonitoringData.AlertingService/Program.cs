using MonitoringData.AlertingService;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services;
using MonitoringData.Infrastructure.Services.DataAccess;

var builder=Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((context, configuration) => {
    configuration.AddJsonFile(MonitorDatabaseSettings.FileName, optional: false, reloadOnChange: true);
});

builder.ConfigureServices((context, services) => {
    services.Configure<MonitorDatabaseSettings>(context.Configuration.GetSection(MonitorDatabaseSettings.SectionName));
    services.AddSingleton<IAlertRepo,AlertRepo>();
    services.AddTransient<IAlertService, AlertService>();
    services.AddHostedService<Worker>();
});


IHost host = builder.Build();

await host.RunAsync();
