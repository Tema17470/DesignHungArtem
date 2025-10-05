using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using SmartGreenhouse.Application.Services;
using SmartGreenhouse.Infrastructure.Data;
using Application.DeviceIntegration; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cs = builder.Configuration.GetConnectionString("Default")
         ?? "Host=localhost;Port=5432;Database=greenhouse;Username=greenhouse;Password=greenhouse";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));

builder.Services.AddSingleton<SimulatedDeviceFactory>();
builder.Services.AddSingleton<IDeviceFactoryResolver, DeviceFactoryResolver>();
builder.Services.AddScoped<CaptureReadingService>();
builder.Services.AddScoped<ReadingService>();
builder.Services.AddScoped<ReadingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
