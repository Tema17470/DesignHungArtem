using MQTTnet;
using MQTTnet.Server;
using System.Text;
using SmartGreenhouse.Application.Mqtt;
using Microsoft.Extensions.DependencyInjection;

public class MqttBrokerHostedService : IHostedService
{
    private MqttServer? _server;
    private readonly IServiceProvider _serviceProvider;

    public MqttBrokerHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        var options = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883)
            .Build();

        _server = new MqttFactory().CreateMqttServer(options);

        _server.InterceptingPublishAsync += async e =>
        {
            var topic = e.ApplicationMessage.Topic ?? "";
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            // Create a scope to get the scoped handler
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<IEsp32MessageHandler>();
                await handler.HandleAsync(topic, payload, ct);
            }
        };

        await _server.StartAsync();
    }

    public async Task StopAsync(CancellationToken ct)
    {
        if (_server != null)
            await _server.StopAsync();
    }
}