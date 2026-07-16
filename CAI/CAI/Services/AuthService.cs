using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CAI.Models;
using Windows.Storage;

namespace CAI.Services
{
    /// <summary>
    /// Handles user authentication — login, registration, token management.
    /// Tokens are stored in Windows ApplicationData local settings.
    /// </summary>
    public class AuthService
    {
        private const string AccessTokenKey = "cai_access_token";
        private const string RefreshTokenKey = "cai_refresh_token";
        private const string UserDataKey = "cai_user_data";

        private readonly ApiService _api;
        private UserModel? _currentUser;

        public AuthService(ApiService api)
        {
            _api = api;
        }

        /// <summary>The currently authenticated user, or null.</summary>
        public UserModel? CurrentUser => _currentUser;

        /// <summary>Whether the user is currently authenticated.</summary>
        public bool IsAuthenticated => !string.IsNullOrEmpty(GetAccessToken());

        /// <summary>
        /// Log in with email and password. Returns the user on success.
        /// </summary>
        public async Task<UserModel?> LoginAsync(string email, string password)
        {
            var payload = new { email, password };
            var json = await _api.PostAsync("/api/auth/login/", payload, authenticated: false);

            if (json == null) return null;

            var response = JsonConvert.DeserializeObject<LoginResponse>(json);
            if (response == null) return null;

            SaveTokens(response.Access, response.Refresh);
            _currentUser = response.User;
            SaveUserData(_currentUser);

            return _currentUser;
        }

        /// <summary>
        /// Register a new account. Requires a previously verified OTP.
        /// </summary>
        public async Task<UserModel?> RegisterAsync(
            string email, string password, string passwordConfirm,
            string otp, string firstName = "", string lastName = "")
        {
            var payload = new
            {
                email,
                password,
                password_confirm = passwordConfirm,
                otp,
                first_name = firstName,
                last_name = lastName,
            };
            var json = await _api.PostAsync("/api/auth/register/", payload, authenticated: false);

            if (json == null) return null;

            var response = JsonConvert.DeserializeObject<RegisterResponse>(json);
            if (response?.Tokens != null)
            {
                SaveTokens(response.Tokens.AccessToken, response.Tokens.RefreshToken);
                _currentUser = response.User;
                SaveUserData(_currentUser);
            }

            return _currentUser;
        }

        /// <summary>Send OTP to an email address.</summary>
        public async Task<bool> SendOtpAsync(string email)
        {
            var json = await _api.PostAsync("/api/auth/send-otp/",
                new { email }, authenticated: false);
            return json != null;
        }

        /// <summary>Verify an OTP code.</summary>
        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var json = await _api.PostAsync("/api/auth/verify-otp/",
                new { email, otp }, authenticated: false);
            if (json == null) return false;

            dynamic? result = JsonConvert.DeserializeObject(json);
            return result?.verified == true;
        }

        /// <summary>Log out and blacklist the refresh token.</summary>
        public async Task LogoutAsync()
        {
            var refresh = GetRefreshToken();
            if (!string.IsNullOrEmpty(refresh))
            {
                await _api.PostAsync("/api/auth/logout/", new { refresh });
            }
            ClearTokens();
            _currentUser = null;
        }

        /// <summary>
        /// Store JWT tokens received from the WebView2 login flow.
        /// Called by LoginPage after the web login page posts tokens via postMessage.
        /// </summary>
        public void SaveTokensFromWebLogin(string access, string refresh)
        {
            SaveTokens(access, refresh);
        }

        /// <summary>Refresh the access token using the stored refresh token.</summary>
        public async Task<bool> RefreshTokenAsync()
        {
            var refresh = GetRefreshToken();
            if (string.IsNullOrEmpty(refresh)) return false;

            var json = await _api.PostAsync("/api/auth/token/refresh/",
                new { refresh }, authenticated: false);

            if (json == null) return false;

            var tokens = JsonConvert.DeserializeObject<AuthTokens>(json);
            if (tokens?.IsValid == true)
            {
                SaveTokens(tokens.AccessToken, tokens.RefreshToken);
                return true;
            }

            return false;
        }

        /// <summary>Load user data from local storage on app startup.</summary>
        public void LoadCachedUser()
        {
            var settings = ApplicationData.Current.LocalSettings;
            var userData = settings.Values[UserDataKey] as string;
            if (!string.IsNullOrEmpty(userData))
            {
                _currentUser = JsonConvert.DeserializeObject<UserModel>(userData);
            }
        }

        // --- Token Storage ---

        public string? GetAccessToken()
        {
            return ApplicationData.Current.LocalSettings.Values[AccessTokenKey] as string;
        }

        public string? GetRefreshToken()
        {
            return ApplicationData.Current.LocalSettings.Values[RefreshTokenKey] as string;
        }

        private void SaveTokens(string access, string refresh)
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values[AccessTokenKey] = access;
            settings.Values[RefreshTokenKey] = refresh;
        }

        private void SaveUserData(UserModel? user)
        {
            if (user == null) return;
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values[UserDataKey] = JsonConvert.SerializeObject(user);
        }

        private void ClearTokens()
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values.Remove(AccessTokenKey);
            settings.Values.Remove(RefreshTokenKey);
            settings.Values.Remove(UserDataKey);
        }
    }
}
