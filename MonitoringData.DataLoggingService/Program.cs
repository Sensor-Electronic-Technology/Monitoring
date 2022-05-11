using MonitoringData.DataLoggingService;
using MonitoringData.Infrastructure.Model;
using MonitoringData.Infrastructure.Services;
using MonitoringData.Infrastructure.Services.DataAccess;
using MassTransit;
using MonitoringSystem.Shared.Contracts;
using MonitoringSystem.Shared.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;


var builder = WebApplication.CreateBuilder(args);
//EndpointConvention.Map<EmailContract>(new Uri("rabbitmq://172.20.3.28:5672/email_processing"));
builder.Configuration.AddJsonFile(MonitorDatabaseSettings.FileName, optional: true, reloadOnChange: true);
builder.Services.Configure<MonitorDatabaseSettings>(builder.Configuration.GetSection(MonitorDatabaseSettings.SectionName));
var hub = builder.Configuration.GetSection(MonitorDatabaseSettings.SectionName).Get<MonitorDatabaseSettings>().HubName;
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

var app = builder.Build();
app.MapHub<MonitorHub>($"/hubs/{hub}");
await app.RunAsync();
