using MonitoringWeb.ControlService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Worker>();
var app = builder.Build();
await app.RunAsync();