using System;
using System.Threading.Tasks;

namespace CAI.Services
{
    /// <summary>
    /// Speech-to-text service stub.
    /// Integration point for System.Speech, Whisper ONNX, or Azure Speech.
    /// </summary>
    public class SpeechService
    {
        public event EventHandler<string>? OnTranscription;
        public event EventHandler<bool>? OnListeningChanged;

        public bool IsListening { get; private set; }

        /// <summary>Start listening for speech input.</summary>
        public Task StartListeningAsync()
        {
            IsListening = true;
            OnListeningChanged?.Invoke(this, true);
            // TODO: Integrate Whisper ONNX or System.Speech here
            return Task.CompletedTask;
        }

        /// <summary>Stop listening.</summary>
        public Task StopListeningAsync()
        {
            IsListening = false;
            OnListeningChanged?.Invoke(this, false);
            return Task.CompletedTask;
        }

        /// <summary>Simulate a transcription result (for testing).</summary>
        public void SimulateTranscription(string text)
        {
            OnTranscription?.Invoke(this, text);
        }
    }
}
