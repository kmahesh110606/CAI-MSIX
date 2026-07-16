using System;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CAI.UI
{
    /// <summary>
    /// Animated splash page showing the CAI logo with audio wave bars.
    /// All animations are driven from code-behind to avoid WinUI 3 Storyboard
    /// TargetName resolution issues with transforms inside property elements.
    /// After ~2.5 seconds, navigates to the appropriate screen.
    /// </summary>
    public sealed partial class SplashPage : Page
    {
        private DispatcherTimer? _waveTimer;
        private DispatcherTimer? _navTimer;
        private double _wavePhase;
        private bool _navigated;

        // Cache the wave bar ScaleTransforms for the animation loop
        private ScaleTransform?[]? _barScales;

        // Wave bar base heights to create varied animation
        private static readonly double[] BarSpeeds = { 1.3, 1.7, 1.5, 1.9, 1.6, 1.4 };

        public SplashPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Cache bar scale transforms
            _barScales = new ScaleTransform?[] {
                Bar1Scale, Bar2Scale, Bar3Scale, Bar4Scale, Bar5Scale, Bar6Scale
            };

            // Animate logo fade-in via property changes
            StartLogoAnimation();

            // Start wave bar animation via timer (not Storyboard)
            _waveTimer = new DispatcherTimer();
            _waveTimer.Interval = TimeSpan.FromMilliseconds(30);
            _waveTimer.Tick += WaveTimer_Tick;
            _waveTimer.Start();

            // Auto-navigate after 2.5 seconds
            _navTimer = new DispatcherTimer();
            _navTimer.Interval = TimeSpan.FromMilliseconds(2500);
            _navTimer.Tick += NavTimer_Tick;
            _navTimer.Start();
        }

        private void StartLogoAnimation()
        {
            // Simple fade-in: set properties directly with a short delay cascade
            LogoContainer.Opacity = 1;
            LogoScale.ScaleX = 1;
            LogoScale.ScaleY = 1;

            // Show subtitle with slight delay using a timer
            var subtitleTimer = new DispatcherTimer();
            subtitleTimer.Interval = TimeSpan.FromMilliseconds(600);
            subtitleTimer.Tick += (s, e) =>
            {
                subtitleTimer.Stop();
                SubtitleText.Opacity = 0.5;
                SubtitleTranslate.Y = 0;
            };
            subtitleTimer.Start();
        }

        private void WaveTimer_Tick(object? sender, object e)
        {
            if (_barScales == null) return;

            _wavePhase += 0.08;
            for (int i = 0; i < _barScales.Length; i++)
            {
                if (_barScales[i] != null)
                {
                    // Sine wave with offset per bar for varied animation
                    double scale = 0.15 + 0.85 * (0.5 + 0.5 * Math.Sin(_wavePhase * BarSpeeds[i] + i * 0.8));
                    _barScales[i]!.ScaleY = scale;
                }
            }
        }

        private void NavTimer_Tick(object? sender, object e)
        {
            if (_navigated) return;
            _navigated = true;

            // Stop all timers first
            _waveTimer?.Stop();
            _waveTimer = null;
            _navTimer?.Stop();
            _navTimer = null;

            // Defer navigation to the next dispatcher cycle so all timer
            // callbacks complete before the window is destroyed
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
            {
                try
                {
                    if (App.Config.IsFirstLaunch)
                    {
                        App.NavigateToOnboarding();
                    }
                    else if (!App.Auth.IsAuthenticated)
                    {
                        App.NavigateToLogin();
                    }
                    else if (!App.Config.IsModelSetupComplete)
                    {
                        App.NavigateToSetup();
                    }
                    else
                    {
                        App.NavigateToMainWindow();
                    }
                }
                catch (Exception)
                {
                    // Window may have been closed already — ignore
                }
            });
        }
    }
}
