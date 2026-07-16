using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CAI.UI
{
    public sealed partial class AssistantOverlay : Page
    {
        private string _transcription = string.Empty;

        public AssistantOverlay()
        {
            InitializeComponent();

            // Subscribe to speech events
            App.Speech.OnTranscription += (s, text) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    _transcription = text;
                    TranscriptionText.Text = text;
                    ExecuteButton.IsEnabled = !string.IsNullOrEmpty(text);
                    StatusText.Text = "Ready";
                    ListeningIndicator.IsActive = false;
                });
            };
        }

        private async void Execute_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_transcription)) return;

            StatusText.Text = "Executing...";
            ExecuteButton.IsEnabled = false;

            var (success, result) = await App.TaskExecutor.ExecuteAsync(_transcription, "system");

            StatusText.Text = success ? "Done ✓" : "Failed ✗";
            TranscriptionText.Text = result;

            // Auto-close after 2 seconds
            await System.Threading.Tasks.Task.Delay(2000);
            // The parent window should close this overlay
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _ = App.Speech.StopListeningAsync();
            // The parent window should close this overlay
        }
    }
}
