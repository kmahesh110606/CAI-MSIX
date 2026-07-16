using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CAI.UI
{
    /// <summary>
    /// Model download setup page. Shows available open-source models with
    /// system requirements and simulated download progress.
    /// </summary>
    public sealed partial class SetupPage : Page
    {
        private readonly HashSet<string> _downloadedModels = new();
        private readonly Dictionary<string, (Button Btn, ProgressBar Bar)> _modelControls = new();

        public SetupPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Register model controls for easy lookup
            _modelControls["phi-3-mini-onnx"] = (DownloadBtn1, Progress1);
            _modelControls["tinyllama-gguf"] = (DownloadBtn2, Progress2);
            _modelControls["mistral-7b-gguf"] = (DownloadBtn3, Progress3);
            _modelControls["llama-3.2-3b-gguf"] = (DownloadBtn4, Progress4);

            // Detect system RAM and show recommendation
            DetectSystemInfo();

            // Restore already-downloaded models from settings
            var alreadyDownloaded = App.Config.Settings.DownloadedModels;
            if (alreadyDownloaded != null)
            {
                foreach (var modelId in alreadyDownloaded)
                {
                    _downloadedModels.Add(modelId);
                    if (_modelControls.TryGetValue(modelId, out var controls))
                    {
                        controls.Btn.Content = "✓ INSTALLED";
                        controls.Btn.IsEnabled = false;
                        controls.Bar.Visibility = Visibility.Collapsed;
                    }
                }
                UpdateContinueState();
            }
        }

        private void DetectSystemInfo()
        {
            try
            {
                // Use GC to get approximate available memory
                long totalMemoryBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
                double totalMemoryGB = totalMemoryBytes / (1024.0 * 1024.0 * 1024.0);

                string recommendation;
                if (totalMemoryGB >= 16)
                    recommendation = $"System RAM: ~{totalMemoryGB:F0} GB — All models supported";
                else if (totalMemoryGB >= 8)
                    recommendation = $"System RAM: ~{totalMemoryGB:F0} GB — Recommended: Phi-3 Mini or Llama 3.2 3B";
                else if (totalMemoryGB >= 4)
                    recommendation = $"System RAM: ~{totalMemoryGB:F0} GB — Recommended: Phi-3 Mini or TinyLlama";
                else
                    recommendation = $"System RAM: ~{totalMemoryGB:F0} GB — Recommended: TinyLlama";

                SystemInfoText.Text = recommendation;
            }
            catch
            {
                SystemInfoText.Text = "Could not detect system specifications";
            }
        }

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not string modelId) return;
            if (_downloadedModels.Contains(modelId)) return;

            // Get the progress bar for this model
            if (!_modelControls.TryGetValue(modelId, out var controls)) return;

            // Disable button, show progress
            btn.IsEnabled = false;
            btn.Content = "DOWNLOADING...";
            controls.Bar.Visibility = Visibility.Visible;
            controls.Bar.Value = 0;

            // Simulate download progress
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            double progress = 0;
            timer.Tick += (s, args) =>
            {
                progress += (100 - progress) * 0.04 + 0.5; // Ease-out progress
                if (progress >= 99.5)
                {
                    progress = 100;
                    timer.Stop();

                    // Mark as downloaded
                    _downloadedModels.Add(modelId);
                    App.Config.UpdateSetting(s =>
                    {
                        if (!s.DownloadedModels.Contains(modelId))
                            s.DownloadedModels.Add(modelId);
                    });

                    btn.Content = "✓ INSTALLED";
                    controls.Bar.Visibility = Visibility.Collapsed;
                    UpdateContinueState();
                }
                controls.Bar.Value = progress;
            };
            timer.Start();

            await System.Threading.Tasks.Task.CompletedTask; // Keep async signature
        }

        private void UpdateContinueState()
        {
            bool hasModel = _downloadedModels.Count > 0;
            ContinueButton.IsEnabled = hasModel;

            if (hasModel)
            {
                DownloadStatusText.Text = $"{_downloadedModels.Count} model(s) installed";

                // Set the first downloaded model as preferred
                foreach (var modelId in _downloadedModels)
                {
                    App.Config.UpdateSetting(s => s.PreferredModel = modelId);
                    break;
                }
            }
            else
            {
                DownloadStatusText.Text = "Download at least one model to continue";
            }
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            App.Config.CompleteModelSetup();
            App.NavigateToMainWindow();
        }
    }
}
