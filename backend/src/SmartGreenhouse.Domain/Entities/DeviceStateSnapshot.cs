using System;

namespace SmartGreenhouse.Domain.Entities
{
    public class DeviceStateSnapshot
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public string StateName { get; set; } = default!;

        public DateTime EnteredAt { get; set; }

        public string? Notes { get; set; }
    }
}
