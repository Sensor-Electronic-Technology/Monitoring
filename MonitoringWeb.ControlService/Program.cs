using MonitoringSystem.Shared.Services;
using MonitoringWeb.ControlService;
using MonitoringWeb.ControlService.Data;
using MonitoringWeb.ControlService.Hubs;
using MonitoringWeb.ControlService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MonitorControlSettings>(builder.Configuration.GetSection(nameof(MonitorControlSettings)));
builder.Services.AddSignalR();
builder.Services.AddSingleton<IModbusService, ModbusService>();
builder.Services.AddSingleton<IMonitorDeviceService, MonitorDeviceService>();
builder.Services.AddSingleton<MonitorControlHub>();
builder.Services.AddHostedService<HubService>();

var app = builder.Build();
app.MapHub<MonitorControlHub>("/hubs/controlhub");
await app.RunAsync();