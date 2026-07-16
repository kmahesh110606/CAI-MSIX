using System;
using System.Threading.Tasks;

namespace CAI.Services
{
    /// <summary>
    /// Wake word detection service stub.
    /// Integration point for Picovoice Porcupine or custom hotword detection.
    /// </summary>
    public class WakeWordService
    {
        public event EventHandler? OnWakeWordDetected;
        public event EventHandler<bool>? OnActiveChanged;

        public bool IsActive { get; private set; }

        /// <summary>Start listening for the wake word.</summary>
        public Task StartAsync()
        {
            IsActive = true;
            OnActiveChanged?.Invoke(this, true);
            // TODO: Integrate Picovoice Porcupine or custom wake word here
            return Task.CompletedTask;
        }

        /// <summary>Stop wake word detection.</summary>
        public Task StopAsync()
        {
            IsActive = false;
            OnActiveChanged?.Invoke(this, false);
            return Task.CompletedTask;
        }

        /// <summary>Simulate wake word detection (for testing).</summary>
        public void SimulateWakeWord()
        {
            OnWakeWordDetected?.Invoke(this, EventArgs.Empty);
        }
    }
}
