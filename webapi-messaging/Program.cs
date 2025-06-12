using webapi_messaging.Configurations.Services;
using webapi_messaging.Configurations.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigApiVersion();

//log4net
builder.Logging.AddLog4Net("log4net.config");

// Add services to the container.
builder.Services.Register();
builder.Services.AllowCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.AddSwaggerGenExt();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerExt();
}

app.ConfigureWebsocket();
app.ConfigSignalR();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
