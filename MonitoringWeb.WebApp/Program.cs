using Append.Blazor.Sidepanel;
using BlazorSpinner;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Services;
using MonitoringWeb.WebApp.Data;
using MonitoringWeb.WebApp.Hubs;
using MonitoringWeb.WebApp.Services;

//github action deploy website test 1
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDevExpressBlazor();
builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});
builder.Services.AddHostedService<AmmoniaHubService>();
#region API Configuration
if (builder.Services.All(x => x.ServiceType != typeof(HttpClient))) {
    builder.Services.AddScoped<HttpClient>(s => {
        var uriHelper = s.GetRequiredService<NavigationManager>();
        return new HttpClient {
            BaseAddress = new Uri("http://configapi"),
        };
    });
}
builder.Services.AddScoped<ConfigApiClient>();
#endregion
#region WebsiteConfiguration
builder.Services.Configure<MonitorWebsiteSettings>(builder.Configuration.GetSection(nameof(MonitorWebsiteSettings)));
builder.Services.AddSingleton<WebsiteConfigurationProvider>();
var connectionString = builder.Configuration.GetSection(nameof(MonitorWebsiteSettings))
    .Get<MonitorWebsiteSettings>()
    ?.ConnectionString;
builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
#endregion
#region ValueChanged UI Services
builder.Services.AddSingleton<SelectionChanged<ModbusDeviceDto>>();
builder.Services.AddSingleton<SelectionChanged<ChannelDto>>();
builder.Services.AddSingleton<ValueChanged<DateRange>>();
builder.Services.AddSingleton<ValueChanged<BulkGasType>>();
builder.Services.AddSingleton<ValueChanged<TankScale>>();
#endregion
#region BulkGas Services
builder.Services.AddSingleton<UsageService>();
builder.Services.AddScoped<BulkGasProvider>();
builder.Services.AddScoped<BulkH2CalcService>();
#endregion
#region AmmoniaServices
builder.Services.AddSingleton<AmmoniaController>();
builder.Services.AddSingleton<AmmoniaDataService>();
#endregion
#region File Services
builder.Services.AddSingleton<FileHandlerService>();
builder.Services.AddControllers();
builder.Services.AddBlazorDownloadFile();
#endregion
#region General Services
builder.Services.AddTransient<PlotDataService>();
builder.Services.AddSingleton<LatestAlertService>();
builder.Services.AddScoped<SpinnerService>();
builder.Services.AddSidepanel();
#endregion

builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
/*app.UseDevExpressBlazorWasmMasksStaticFiles();*/
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
var websiteConfigProvider = app.Services.GetService<WebsiteConfigurationProvider>();
if (websiteConfigProvider is not null) {
    await websiteConfigProvider.Load();
} else {
    throw new Exception("Error: could not resolve WebsiteConfigurationProvider");
}
app.MapHub<BulkGasHub>("/bulkgashub");
app.MapHub<AmmoniaHub>("/tank-weights");
app.MapFallbackToPage("/_Host");
app.MapControllers();
app.Run();