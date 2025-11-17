using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;

namespace SmartGreenhouse.Application.Adapters.Notifications
{
    public class WebhookNotificationAdapter : INotificationAdapter
    {
        private readonly HttpClient _client;
        private readonly string _webhookUrl;

        public WebhookNotificationAdapter(HttpClient client, string webhookUrl)
        {
            _client = client;
            _webhookUrl = webhookUrl;
        }

        public async Task NotifyAsync(int deviceId, string title, string message, CancellationToken ct = default)
        {
            var payload = new
            {
                DeviceId = deviceId,
                Title = title,
                Message = message
            };
            await _client.PostAsJsonAsync(_webhookUrl, payload, ct);
        }
    }
}
