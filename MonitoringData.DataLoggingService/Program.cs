using MonitoringData.DataLoggingService;
using MonitoringData.Infrastructure.Services;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostContext, configuration) => {
    configuration.AddJsonFile("dbSettings.json", optional: true, reloadOnChange: true);
});

builder.ConfigureServices((hostContext, services) => {
    services.Configure<ServiceConfiguration>(hostContext.Configuration.GetSection(ServiceConfiguration.Section));
    ServiceConfiguration sConfig = new();
    hostContext.Configuration.GetSection(ServiceConfiguration.Section).Bind(sConfig);

});

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
