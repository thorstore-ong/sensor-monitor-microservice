using SensorMonitor.Models;
using SensorMonitor.Normalize;
using SensorMonitor.Services;
using SensorMonitor.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

var channel = Channel.CreateBounded<SensorTelemetry>( new BoundedChannelOptions(100)
{
    SingleReader = true,
    SingleWriter = false
});


// Register EF Core with PostgreSQL
builder.Services.AddDbContext<TelemetryDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );


//Registering Channel and Background Service
builder.Services.AddSingleton(channel);
builder.Services.AddHostedService<TelemetryProcessor>();

//Registering Sensor Service
builder.Services.AddScoped<SensorService>();
builder.Services.AddSingleton<TelemetryNormalizer>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine(
    builder.Configuration.GetConnectionString("DefaultConnection")
    );

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
