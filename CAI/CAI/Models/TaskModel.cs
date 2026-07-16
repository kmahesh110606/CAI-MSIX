using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CAI.Models
{
    /// <summary>
    /// Represents a cross-device task.
    /// </summary>
    public class TaskModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; } = string.Empty;

        [JsonProperty("command_type")]
        public string CommandType { get; set; } = "system";

        [JsonProperty("priority")]
        public string Priority { get; set; } = "normal";

        [JsonProperty("status")]
        public string Status { get; set; } = "pending";

        [JsonProperty("source_device")]
        public Guid SourceDeviceId { get; set; }

        [JsonProperty("target_device")]
        public Guid TargetDeviceId { get; set; }

        [JsonProperty("result")]
        public string? Result { get; set; }

        [JsonProperty("error_message")]
        public string? ErrorMessage { get; set; }

        [JsonProperty("inference_data")]
        public Dictionary<string, object>? InferenceData { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("completed_at")]
        public DateTime? CompletedAt { get; set; }
    }
}
