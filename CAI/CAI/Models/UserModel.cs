using System;
using Newtonsoft.Json;

namespace CAI.Models
{
    /// <summary>
    /// Represents the authenticated user.
    /// </summary>
    public class UserModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonProperty("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("date_joined")]
        public DateTime? DateJoined { get; set; }

        public string DisplayName => string.IsNullOrEmpty(FirstName)
            ? Email
            : $"{FirstName} {LastName}".Trim();
    }
}
