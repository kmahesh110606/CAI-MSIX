using System;
using Newtonsoft.Json;

namespace CAI.Models
{
    /// <summary>
    /// Represents a chat message in a conversation.
    /// </summary>
    public class ChatMessageModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("session")]
        public Guid SessionId { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; } = "user";

        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("synced")]
        public bool Synced { get; set; }

        /// <summary>
        /// True if this message is from the user (for UI alignment).
        /// </summary>
        public bool IsUser => Role == "user";
    }
}
