using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;

namespace CAI.Services
{
    /// <summary>
    /// HTTP client wrapper for all REST API calls to the Django server.
    /// Handles JWT authentication, token refresh, and error handling.
    /// </summary>
    public class ApiService
    {
        private readonly HttpClient _client;
        private string _baseUrl;

        public ApiService()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _client.Timeout = TimeSpan.FromSeconds(30);

            // Load server URL from settings
            var settings = ApplicationData.Current.LocalSettings;
            _baseUrl = settings.Values["cai_server_url"] as string
                ?? "http://127.0.0.1:8000";
        }

        /// <summary>Update the server base URL.</summary>
        public void SetBaseUrl(string url)
        {
            _baseUrl = url.TrimEnd('/');
            ApplicationData.Current.LocalSettings.Values["cai_server_url"] = _baseUrl;
        }

        /// <summary>Perform a GET request.</summary>
        public async Task<string?> GetAsync(string endpoint, bool authenticated = true)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + endpoint);
                if (authenticated) AttachToken(request);

                var response = await _client.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && authenticated)
                {
                    if (await TryRefreshToken())
                    {
                        request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + endpoint);
                        AttachToken(request);
                        response = await _client.SendAsync(request);
                    }
                }

                return response.IsSuccessStatusCode
                    ? await response.Content.ReadAsStringAsync()
                    : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>Perform a POST request with JSON body.</summary>
        public async Task<string?> PostAsync(string endpoint, object? body = null, bool authenticated = true)
        {
            try
            {
                var json = body != null ? JsonConvert.SerializeObject(body) : "{}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + endpoint)
                {
                    Content = content,
                };
                if (authenticated) AttachToken(request);

                var response = await _client.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && authenticated)
                {
                    if (await TryRefreshToken())
                    {
                        content = new StringContent(json, Encoding.UTF8, "application/json");
                        request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + endpoint)
                        {
                            Content = content,
                        };
                        AttachToken(request);
                        response = await _client.SendAsync(request);
                    }
                }

                return response.IsSuccessStatusCode
                    ? await response.Content.ReadAsStringAsync()
                    : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>Perform a PATCH request.</summary>
        public async Task<string?> PatchAsync(string endpoint, object? body = null)
        {
            try
            {
                var json = body != null ? JsonConvert.SerializeObject(body) : "{}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), _baseUrl + endpoint)
                {
                    Content = content,
                };
                AttachToken(request);

                var response = await _client.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? await response.Content.ReadAsStringAsync()
                    : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>Perform a DELETE request.</summary>
        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, _baseUrl + endpoint);
                AttachToken(request);

                var response = await _client.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // --- Helpers ---

        private void AttachToken(HttpRequestMessage request)
        {
            var token = ApplicationData.Current.LocalSettings.Values["cai_access_token"] as string;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<bool> TryRefreshToken()
        {
            var refresh = ApplicationData.Current.LocalSettings.Values["cai_refresh_token"] as string;
            if (string.IsNullOrEmpty(refresh)) return false;

            try
            {
                var json = JsonConvert.SerializeObject(new { refresh });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(_baseUrl + "/api/auth/token/refresh/", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    dynamic? tokens = JsonConvert.DeserializeObject(result);
                    if (tokens?.access != null)
                    {
                        var settings = ApplicationData.Current.LocalSettings;
                        settings.Values["cai_access_token"] = (string)tokens.access;
                        if (tokens.refresh != null)
                            settings.Values["cai_refresh_token"] = (string)tokens.refresh;
                        return true;
                    }
                }
            }
            catch (Exception) { }

            return false;
        }
    }
}
