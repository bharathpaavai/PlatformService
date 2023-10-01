using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;
//test
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure the HTTP request pipeline.

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMvc();

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

var configValue = builder.Configuration.GetValue<string>("CommandService");

Console.WriteLine($"--> CommandService EndPoint {configValue}");
if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlafformsConn")));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}

var app = builder.Build();

app.MapControllers();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();

