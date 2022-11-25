using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringWeb.WebApp.Data;
using MonitoringWeb.WebApp.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDevExpressBlazor();
builder.Services.AddDevExpressBlazorWasmMasks();
builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});
builder.Services.Configure<MonitorWebsiteSettings>(builder.Configuration.GetSection(nameof(MonitorWebsiteSettings)));
if (builder.Services.All(x => x.ServiceType != typeof(HttpClient))) {
    // Setup HttpClient for server side in a client side compatible fashion
    builder.Services.AddScoped<HttpClient>(s => {
        // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
        var uriHelper = s.GetRequiredService<NavigationManager>();
        return new HttpClient {
            BaseAddress = new Uri("http://172.20.3.203"),
        };
    });
}
builder.Services.AddSingleton<PlotDataService>();
builder.Services.AddSingleton<LatestAlertService>();
var connectionString = builder.Configuration.GetSection(nameof(MonitorWebsiteSettings))
    .Get<MonitorWebsiteSettings>()
    .ConnectionString;
builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
builder.Services.AddSingleton<WebsiteConfigurationProvider>();
builder.Services.AddScoped<ConfigApiClient>();
builder.Services.AddSingleton<SelectionChanged<ModbusDeviceDto>>();
builder.Services.AddSingleton<SelectionChanged<ChannelDto>>();
builder.Services.AddSingleton<ValueChanged<DateRange>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseDevExpressBlazorWasmMasksStaticFiles();
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
app.MapFallbackToPage("/_Host");

app.Run();