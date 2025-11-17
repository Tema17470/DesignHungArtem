using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using SmartGreenhouse.Application.State;

namespace SmartGreenhouse.Application.Adapters.Actuators
{
    public class HttpActuatorAdapter : IActuatorAdapter
    {
        private readonly HttpClient _client;

        public HttpActuatorAdapter(HttpClient client)
        {
            _client = client;
        }

        public async Task ApplyAsync(int deviceId, IReadOnlyList<ActuatorCommand> commands, CancellationToken ct = default)
        {
            var url = $"/devices/{deviceId}/actuators";
            var response = await _client.PostAsJsonAsync(url, commands, ct);
            response.EnsureSuccessStatusCode();
        }
    }
}
