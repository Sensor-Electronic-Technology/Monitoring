using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.Negotiate;
using MonitoringConfig.Data.Model;
using FastEndpoints.ClientGen;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc(s=>
{
    s.DocumentName = "v1";
    s.GenerateKnownTypes = false;
},shortSchemaNames:true);
builder.Services.AddDbContext<MonitorContext>();




var app = builder.Build();

app.UseFastEndpoints(s =>
{
    s.Endpoints.ShortNames = true;
});
await app.GenerateClientsAndExitAsync(
    documentName: "v1", //must match doc name above
    destinationPath: @"C:\Generated",
    csSettings: c =>
    {
        c.ClassName = "ApiClient";
        c.GenerateDtoTypes = false;
    },
    tsSettings: c=>c.ClassName="apiTS");
app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());
app.Run();
