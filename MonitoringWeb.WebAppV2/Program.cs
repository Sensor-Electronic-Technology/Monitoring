using MonitoringData.Infrastructure.Services.DataAccess;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Driver;
using MonitoringWeb.WebAppV2.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDevExpressBlazor();
builder.Services.Configure<WebsiteSettings>(builder.Configuration.GetSection(nameof(WebsiteSettings)));
builder.Services.AddSingleton<DataDownload>();
builder.Services.AddSingleton<PlotDataService>();
builder.Services.AddSingleton<LatestAlertService>();
var connectionString = builder.Configuration.GetSection(nameof(WebsiteSettings)).Get<WebsiteSettings>().ConnectionString;
builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
builder.Services.AddSingleton<SettingsService>();
builder.Services.AddDevExpressBlazorWasmMasks();

builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});

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
app.MapFallbackToPage("/_Host");
app.Run();