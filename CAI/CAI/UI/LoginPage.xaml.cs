using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace CAI.UI
{
    /// <summary>
    /// Login page that hosts a WebView2 pointing at the Django website login page.
    /// When the user logs in on the web page, the JWT tokens are sent back
    /// via window.chrome.webview.postMessage and stored locally.
    /// After login, routes to SetupPage if model setup is not complete.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private string _loginUrl;

        public LoginPage()
        {
            InitializeComponent();

            // Build the login URL from the configured server base URL
            var serverUrl = (Windows.Storage.ApplicationData.Current.LocalSettings
                .Values["cai_server_url"] as string ?? "http://127.0.0.1:8000").TrimEnd('/');
            _loginUrl = serverUrl + "/login/";
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeWebView();
        }

        private async System.Threading.Tasks.Task InitializeWebView()
        {
            try
            {
                // Ensure the WebView2 core environment is ready
                await LoginWebView.EnsureCoreWebView2Async();

                // Listen for messages from the web page (LOGIN_SUCCESS)
                LoginWebView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

                // Hide loading overlay once initial navigation completes
                LoginWebView.CoreWebView2.NavigationCompleted += OnNavigationCompleted;

                // Navigate to the login page
                LoginWebView.CoreWebView2.Navigate(_loginUrl);
            }
            catch (Exception ex)
            {
                ShowError($"WebView2 failed to initialize: {ex.Message}");
            }
        }

        private void OnNavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                // Hide loading, show webview
                LoadingOverlay.Visibility = Visibility.Collapsed;
                ErrorOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                ShowError("Could not reach the CAI server. Please check your connection and server URL in Settings.");
            }
        }

        private void OnWebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            try
            {
                var messageJson = args.WebMessageAsJson;
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<WebLoginMessage>(messageJson);

                if (message != null && message.Type == "LOGIN_SUCCESS"
                    && !string.IsNullOrEmpty(message.Access)
                    && !string.IsNullOrEmpty(message.Refresh))
                {
                    // Store tokens
                    App.Auth.SaveTokensFromWebLogin(message.Access, message.Refresh);

                    // Route based on model setup state
                    if (!App.Config.IsModelSetupComplete)
                    {
                        App.NavigateToSetup();
                    }
                    else
                    {
                        App.NavigateToMainWindow();
                    }
                }
            }
            catch (Exception)
            {
                // Ignore malformed messages
            }
        }

        private void ShowError(string message)
        {
            LoadingOverlay.Visibility = Visibility.Collapsed;
            ErrorMessage.Text = message;
            ErrorOverlay.Visibility = Visibility.Visible;
        }

        private async void Retry_Click(object sender, RoutedEventArgs e)
        {
            ErrorOverlay.Visibility = Visibility.Collapsed;
            LoadingOverlay.Visibility = Visibility.Visible;
            await InitializeWebView();
        }

        /// <summary>
        /// Deserialization model for the postMessage payload from login.html.
        /// </summary>
        private class WebLoginMessage
        {
            [Newtonsoft.Json.JsonProperty("type")]
            public string Type { get; set; } = string.Empty;

            [Newtonsoft.Json.JsonProperty("access")]
            public string Access { get; set; } = string.Empty;

            [Newtonsoft.Json.JsonProperty("refresh")]
            public string Refresh { get; set; } = string.Empty;
        }
    }
}
