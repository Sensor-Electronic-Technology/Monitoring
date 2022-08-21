using FastEndpoints;
using FastEndpoints.Swagger;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();

builder.Services.AddSingleton<IMongoClient>(new MongoClient(config.GetValue<string>("Database:ConnectionString")));

var app = builder.Build();
app.UseFastEndpoints();
app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());
app.Run();

/*// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

/*app.UseHttpsRedirection();*/






