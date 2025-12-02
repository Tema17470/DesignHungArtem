using MQTTnet;
using MQTTnet.Server;
using System.Text;
using SmartGreenhouse.Application.Mqtt;
using Microsoft.Extensions.Hosting;

namespace SmartGreenhouse.Api.RealTime;

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
            var payloadBytes = e.ApplicationMessage.Payload;
            var payload = payloadBytes == null ? string.Empty : Encoding.UTF8.GetString(payloadBytes);

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
