using Append.Blazor.Sidepanel;
using MonitoringData.Infrastructure.Services.DataAccess;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Driver;
using MonitoringData.Infrastructure.MongoConfiguration;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Services;
using MonitoringWeb.WebAppV2.Data;
using MonitoringWeb.WebAppV2.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDevExpressBlazor();
builder.Services.Configure<MonitorWebsiteSettings>(builder.Configuration.GetSection(nameof(MonitorWebsiteSettings)));
builder.Services.Configure<ManagedDevice>(builder.Configuration.GetSection("managed_devices"));
builder.Services.AddSingleton<DataDownload>();
builder.Services.AddSingleton<PlotDataService>();
builder.Services.AddSingleton<LatestAlertService>();
var connectionString = builder.Configuration.GetSection(nameof(MonitorWebsiteSettings))
    .Get<MonitorWebsiteSettings>()
    .ConnectionString;
builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
builder.Services.AddSingleton<IMonitorConfigurationProvider, WebsiteConfigurationProvider>();
builder.Services.AddSidepanel();
builder.Services.AddDevExpressBlazorWasmMasks();

builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseDevExpressBlazorWasmMasksStaticFiles();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
var websiteConfigProvider = app.Services.GetService<IMonitorConfigurationProvider>();
if (websiteConfigProvider is not null) {
    await websiteConfigProvider.Load();
} else {
    throw new Exception("Error: could not resolve WebsiteConfigurationProvider");
}
app.MapFallbackToPage("/_Host");
app.Run();