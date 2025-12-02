using MQTTnet;
using MQTTnet.Server;
using System.Text;
using SmartGreenhouse.Application.Mqtt;

public class MqttBrokerHostedService : IHostedService
{
    private MqttServer? _server;
    private readonly IEsp32MessageHandler _handler;

    public MqttBrokerHostedService(IEsp32MessageHandler handler)
    {
        _handler = handler;
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

            await _handler.HandleAsync(topic, payload, ct);
        };

        await _server.StartAsync();
    }

    public async Task StopAsync(CancellationToken ct)
    {
        if (_server != null)
            await _server.StopAsync();
    }
}