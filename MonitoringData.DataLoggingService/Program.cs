using MonitoringData.DataLoggingService;
using MonitoringSystem.Shared.Data;
using MonitoringData.Infrastructure.Services;
using MonitoringData.Infrastructure.Services.DataAccess;
using MassTransit;
using MonitoringSystem.Shared.Contracts;
using MonitoringSystem.Shared.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Data;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringData.Infrastructure.Services.DataLogging;
using MonitoringSystem.Shared.Services;

var builder = WebApplication.CreateBuilder(args);
//EndpointConvention.Map<EmailContract>(new Uri("rabbitmq://172.20.3.28:5672/email_processing"));
builder.Configuration.AddJsonFile(MonitorDatabaseSettings.FileName, optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<MonitorDatabaseSettings>(builder.Configuration.GetSection(MonitorDatabaseSettings.SectionName));
builder.Services.Configure<MonitorEmailSettings>(builder.Configuration.GetSection(nameof(MonitorEmailSettings)));
var hub = builder.Configuration.GetSection(MonitorDatabaseSettings.SectionName).Get<MonitorDatabaseSettings>().HubName;
var serviceType = builder.Configuration.GetSection(ServiceOptions.SectionName).Get<ServiceOptions>().ServiceType;


builder.Services.AddMediator(cfg => {
    cfg.AddConsumer<MonitorBoxLogger>();
});

builder.Services.AddSingleton<IMonitorDataRepo, MonitorDataService>();
builder.Services.AddSingleton<IAlertRepo, AlertRepo>();
builder.Services.AddSingleton<IModbusService, ModbusService>();
builder.Services.AddTransient<IAlertService, AlertService>();
builder.Services.AddSingleton<IEmailService, SmtpEmailService>();
switch (serviceType) {
    case ServiceType.GenericModbus:
        builder.Services.AddSingleton<IDataLogger, ModbusLogger>();
        break;
    case ServiceType.MonitorBox:
        builder.Services.AddSingleton<IDataLogger, MonitorBoxLogger>();
        break;
    case ServiceType.API:
        break;
    case ServiceType.BacNet:
        break;
}

builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<MonitorDBChanges>();
builder.Services.AddSignalR();
var app = builder.Build();
var emailService=app.Services.GetService<IEmailService>();
if (emailService is not null) {
    var success=await emailService.Load();
    if (!success) {
        throw new Exception("Error: Failed to load email recipients");
    }
} else {
    throw new Exception("Error: Could not resolve SettingsService");
}
app.MapHub<MonitorHub>($"/hubs/{hub}");
await app.RunAsync();
