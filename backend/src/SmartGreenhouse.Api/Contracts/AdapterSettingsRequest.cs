namespace SmartGreenhouse.Api.Contracts
{
    public class AdapterSettingsRequest
    {
        public string ActuatorMode { get; set; } = "Simulated";  // "Simulated" | "Http" | "Mqtt"
        public string NotificationMode { get; set; } = "Console"; // "Console" | "Webhook"
        public string? WebhookUrl { get; set; }                  // optional
    }
}
