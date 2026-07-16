using System;
using Microsoft.UI.Xaml;
using CAI.Services;

namespace CAI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// Initializes services and handles first-launch routing.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        // Global service instances
        public static ApiService Api { get; } = new ApiService();
        public static AuthService Auth { get; } = new AuthService(Api);
        public static ConfigService Config { get; } = new ConfigService();
        public static TaskService TaskExecutor { get; } = new TaskService();
        public static WebSocketService WebSocket { get; } = new WebSocketService();
        public static SpeechService Speech { get; } = new SpeechService();
        public static WakeWordService WakeWord { get; } = new WakeWordService();

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Load cached user data
            Auth.LoadCachedUser();

            // Always show splash screen first — it auto-routes based on state
            _window = new Window();
            _window.Title = "CAI";
            _window.ExtendsContentIntoTitleBar = true;
            _window.Content = new UI.SplashPage();
            _window.SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
            _window.Activate();
        }

        /// <summary>Navigate to onboarding (first launch).</summary>
        public static void NavigateToOnboarding()
        {
            var app = (App)Current;
            if (app._window != null)
            {
                app._window.Title = "Welcome to CAI";
                app._window.Content = new UI.OnboardingPage();
            }
        }

        /// <summary>Navigate to login window.</summary>
        public static void NavigateToLogin()
        {
            var app = (App)Current;
            if (app._window != null)
            {
                app._window.Title = "CAI — Sign In";
                app._window.Content = new UI.LoginPage();
            }
        }

        /// <summary>Navigate to model setup page.</summary>
        public static void NavigateToSetup()
        {
            var app = (App)Current;
            if (app._window != null)
            {
                app._window.Title = "CAI — Setup";
                app._window.Content = new UI.SetupPage();
            }
        }

        /// <summary>Navigate to the main window after login/onboarding/setup.</summary>
        public static void NavigateToMainWindow()
        {
            var app = (App)Current;
            var oldWindow = app._window;
            
            app._window = new MainWindow();
            app._window.Activate();

            if (oldWindow != null && oldWindow != app._window)
            {
                oldWindow.Close();
            }
        }
    }
}
