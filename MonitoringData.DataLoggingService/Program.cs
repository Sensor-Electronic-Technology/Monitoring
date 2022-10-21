using MonitoringData.DataLoggingService;
using MonitoringSystem.Shared.Data;
using MonitoringData.Infrastructure.Services;
using MonitoringData.Infrastructure.Services.DataAccess;
using MassTransit;
using MonitoringSystem.Shared.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Data;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringData.Infrastructure.Services.DataLogging;
using MonitoringSystem.Shared.Data.SettingsModel;
using MonitoringSystem.Shared.Services;

var builder = WebApplication.CreateBuilder(args);
//EndpointConvention.Map<EmailContract>(new Uri("rabbitmq://172.20.3.28:5672/email_processing"));
/*builder.Configuration.AddJsonFile(MonitorDatabaseSettings.FileName, optional: false, reloadOnChange: true);*/
builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<MonitorDataLogSettings>(builder.Configuration.GetSection(nameof(MonitorDataLogSettings)));
builder.Services.Configure<MonitorEmailSettings>(builder.Configuration.GetSection(nameof(MonitorEmailSettings)));
//var hub = builder.Configuration.GetSection(MonitorDatabaseSettings.SectionName).Get<MonitorDatabaseSettings>().HubName;
var settings = builder.Configuration.GetSection(nameof(MonitorDataLogSettings)).Get<MonitorDataLogSettings>();
builder.Services.AddMediator(cfg => {
    //cfg.AddConsumer<MonitorBoxLogger>();
    cfg.AddConsumer<Worker>();
});

builder.Services.AddSingleton<DataLogConfigProvider>();
builder.Services.AddSingleton<IMonitorDataRepo, MonitorDataService>();
builder.Services.AddSingleton<IAlertRepo, AlertRepo>();
builder.Services.AddSingleton<IModbusService, ModbusService>();
builder.Services.AddSingleton<IAlertService, AlertService>();
builder.Services.AddSingleton<IEmailService, SmtpEmailService>();
builder.Services.AddSingleton<IMongoClient>(new MongoClient(settings.ConnectionString));
var serviceType = Environment.GetEnvironmentVariable("SERVICE_TYPE");
//var serviceType = nameof(ServiceType.MonitorBox);
Console.WriteLine($"ServiceType: {serviceType}");
switch (serviceType) {
    case nameof(ServiceType.GenericModbus):
        builder.Services.AddSingleton<IDataLogger, ModbusLogger>();
        break;
    case nameof(ServiceType.MonitorBox):
        builder.Services.AddSingleton<IDataLogger, MonitorBoxLogger>();
        break;
    case nameof(ServiceType.API):
        break;
    case nameof(ServiceType.BacNet):
        break;
}
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<MonitorReadingDatabase>();
builder.Services.AddHostedService<MonitorConfigDatabase>();
builder.Services.AddSignalR();
var app = builder.Build();

var dataConfigProvider = app.Services.GetService<DataLogConfigProvider>();
if (dataConfigProvider is not null) {
    var deviceName=Environment.GetEnvironmentVariable("DEVICEID");
    //var deviceName = "epi2";
    if (deviceName is not null) {
        dataConfigProvider.DeviceName = deviceName;
        await dataConfigProvider.Load();
        var emailService=app.Services.GetService<IEmailService>();
        if (emailService is not null) {
            await emailService.Load();
        } else {
            throw new Exception("Error: Could not resolve SettingsService");
        }
    } else {
        throw new Exception("Error: EnvironmentVariable DEVICEID not found");
    }
} else {
    throw new Exception("Error: Could not resolve DataLogConfig");
}
var hub = dataConfigProvider.ManagedDevice.HubName;
app.MapHub<MonitorHub>($"/hubs/{hub}");

await app.RunAsync();
