using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using SmartGreenhouse.Application.Services;
using Application.Abstractions;
using Application.Events;
using Application.Events.Observers;
using SmartGreenhouse.Infrastructure.Data;
using Application.DeviceIntegration;
using Application.Control;
using SmartGreenhouse.Application.State;
using SmartGreenhouse.Application.State.States;
using SmartGreenhouse.Application.Adapters;
using SmartGreenhouse.Application.Adapters.Actuators;
using SmartGreenhouse.Application.Adapters.Notifications;
using SmartGreenhouse.Api.RealTime;
using System.Net.WebSockets;

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

builder.Services.AddScoped<IReadingObserver, LogObserver>();
builder.Services.AddScoped<IReadingObserver, AlertRuleObserver>();
builder.Services.AddScoped<IReadingPublisher, ReadingPublisher>();

builder.Services.AddScoped<HysteresisCoolingStrategy>();
builder.Services.AddScoped<MoistureTopUpStrategy>();
builder.Services.AddScoped<IControlStrategySelector, ControlStrategySelector>();
builder.Services.AddScoped<ControlService>();

// HTTP clients for adapters
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<HttpActuatorAdapter>(client =>
{
    // Default external IoT hub or device controller URL (example)
    client.BaseAddress = new Uri("http://localhost:5055/");
});
builder.Services.AddHttpClient("WebhookClient");

// Adapters
builder.Services.AddSingleton<INotificationAdapter, ConsoleNotificationAdapter>();
builder.Services.AddSingleton<IActuatorAdapter, SimulatedActuatorAdapter>();
builder.Services.AddSingleton<AdapterRegistry>(sp =>
    new AdapterRegistry(
        sp.GetRequiredService<IActuatorAdapter>(),
        sp.GetRequiredService<INotificationAdapter>()
    )
);

// realtime
builder.Services.AddSingleton<LiveReadingHub>();
builder.Services.AddSingleton<SmartGreenhouse.Application.RealTime.IRealTimeNotifier, WebSocketRealTimeNotifier>();

// mqtt broker
builder.Services.AddHostedService<MqttBrokerHostedService>();

// State engine & service
builder.Services.AddScoped<GreenhouseStateEngine>();
builder.Services.AddScoped<StateService>();

// Optionally register concrete states in DI if you want to resolve by name:
builder.Services.AddScoped<IdleState>();
builder.Services.AddScoped<CoolingState>();
builder.Services.AddScoped<IrrigatingState>();
builder.Services.AddScoped<AlarmState>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();

app.Map("/ws/live-readings", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    var hub = context.RequestServices.GetRequiredService<LiveReadingHub>();
    var socket = await context.WebSockets.AcceptWebSocketAsync();

    hub.Register(socket);

    var buffer = new byte[4096];
    while (socket.State == WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close)
            break;
    }

    hub.Unregister(socket);
    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
