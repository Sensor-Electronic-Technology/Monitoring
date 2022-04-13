using MonitoringData.DataLoggingService;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services;
using MonitoringData.Infrastructure.Services.DataAccess;
using MassTransit;
using MonitoringSystem.Shared.Contracts;

var builder = Host.CreateDefaultBuilder(args);

EndpointConvention.Map<EmailContract>(new Uri("rabbitmq://172.20.3.28:5672/email_processing"));

builder.ConfigureAppConfiguration((hostContext, configuration) => {
    configuration.AddJsonFile(MonitorDatabaseSettings.FileName, optional: true, reloadOnChange: true);
});

builder.ConfigureServices((hostContext, services) => {

    services.Configure<MonitorDatabaseSettings>(hostContext.Configuration.GetSection(MonitorDatabaseSettings.SectionName));
    //services.AddMassTransit(bus => {
    //    bus.UsingRabbitMq((context,rabbitcfg) => {
    //        rabbitcfg.Host(new Uri("rabbitmq://172.20.3.28:5672/"), (hostcfg) => {
    //            hostcfg.Username("setiadmin");
    //            hostcfg.Password("Sens0r20471#!");
    //        });
    //    });
    //});
    //services.AddTransient<ISendEndpoint>();
    services.AddSingleton<IMonitorDataRepo,MonitorDataService>();
    services.AddSingleton<IAlertRepo,AlertRepo>();
    services.AddSingleton<IModbusService, ModbusService>();
    services.AddTransient<IAlertService, AlertService>();
    services.AddTransient<IDataLogger,ModbusDataLogger>();
    services.AddHostedService<Worker>();
});

IHost host = builder.Build();

await host.RunAsync();
