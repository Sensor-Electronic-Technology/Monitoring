using MonitoringData.DataLoggingService;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services;
using MonitoringData.Infrastructure.Services.DataAccess;
using MassTransit;
using MonitoringSystem.Shared.Contracts;
using MonitoringSystem.Shared.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

//var builder = Host.CreateDefaultBuilder(args);
var builder = WebApplication.CreateBuilder(args);

//EndpointConvention.Map<EmailContract>(new Uri("rabbitmq://172.20.3.28:5672/email_processing"));

builder.Configuration.AddJsonFile(MonitorDatabaseSettings.FileName, optional: true, reloadOnChange: true);

//builder.ConfigureAppConfiguration((hostContext, configuration) => {
//    configuration.AddJsonFile(MonitorDatabaseSettings.FileName, optional: true, reloadOnChange: true);
//});
builder.Services.Configure<MonitorDatabaseSettings>(builder.Configuration.GetSection(MonitorDatabaseSettings.SectionName));
builder.Services.AddMediator(cfg => {
    cfg.AddConsumer<ModbusDataLogger>();
});
builder.Services.AddSingleton<IMonitorDataRepo, MonitorDataService>();
builder.Services.AddSingleton<IAlertRepo, AlertRepo>();
builder.Services.AddSingleton<IModbusService, ModbusService>();
builder.Services.AddTransient<IAlertService, AlertService>();
builder.Services.AddTransient<IDataLogger, ModbusDataLogger>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<MonitorDBChanges>();
builder.Services.AddSignalR();

//builder.ConfigureServices((hostContext, services) => {

//    services.Configure<MonitorDatabaseSettings>(hostContext.Configuration.GetSection(MonitorDatabaseSettings.SectionName));
//    services.AddMediator(cfg => {
//        cfg.AddConsumer<ModbusDataLogger>();
//    });
//    services.AddSingleton<IMonitorDataRepo,MonitorDataService>();
//    services.AddSingleton<IAlertRepo,AlertRepo>();
//    services.AddSingleton<IModbusService, ModbusService>();
//    services.AddTransient<IAlertService, AlertService>();
//    services.AddTransient<IDataLogger,ModbusDataLogger>();
//    services.AddHostedService<Worker>();
//    services.AddHostedService<MonitorDBChanges>();
//    //services.AddSignalR();
//});


var app = builder.Build();
app.MapHub<MonitorHub>("/hubs/epi2streaming");
await app.RunAsync();
//await app.RunAsync();

//await host.RunAsync();
