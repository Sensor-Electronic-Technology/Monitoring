var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();
//app.MapHub<>($"/hubs/{hub}");
app.Run();