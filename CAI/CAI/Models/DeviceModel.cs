using System;
using Newtonsoft.Json;

namespace CAI.Models
{
    /// <summary>
    /// Represents a registered device in the CAI network.
    /// </summary>
    public class DeviceModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("device_type")]
        public string DeviceType { get; set; } = "desktop";

        [JsonProperty("os")]
        public string OS { get; set; } = "windows";

        [JsonProperty("os_version")]
        public string? OSVersion { get; set; }

        [JsonProperty("app_version")]
        public string? AppVersion { get; set; }

        [JsonProperty("is_online")]
        public bool IsOnline { get; set; }

        [JsonProperty("registered_at")]
        public DateTime RegisteredAt { get; set; }

        [JsonProperty("last_seen")]
        public DateTime LastSeen { get; set; }
    }
}
