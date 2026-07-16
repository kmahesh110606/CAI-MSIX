using Newtonsoft.Json;

namespace CAI.Models
{
    /// <summary>
    /// JWT token pair returned by the authentication API.
    /// </summary>
    public class AuthTokens
    {
        [JsonProperty("access")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("refresh")]
        public string RefreshToken { get; set; } = string.Empty;

        public bool IsValid => !string.IsNullOrEmpty(AccessToken);
    }

    /// <summary>
    /// Login response from the API.
    /// </summary>
    public class LoginResponse
    {
        [JsonProperty("access")]
        public string Access { get; set; } = string.Empty;

        [JsonProperty("refresh")]
        public string Refresh { get; set; } = string.Empty;

        [JsonProperty("user")]
        public UserModel? User { get; set; }
    }

    /// <summary>
    /// Registration response from the API.
    /// </summary>
    public class RegisterResponse
    {
        [JsonProperty("detail")]
        public string Detail { get; set; } = string.Empty;

        [JsonProperty("tokens")]
        public AuthTokens? Tokens { get; set; }

        [JsonProperty("user")]
        public UserModel? User { get; set; }
    }
}
