using System.Collections.Generic;
using Newtonsoft.Json;

namespace CAI.Models
{
    /// <summary>
    /// Application settings persisted locally.
    /// </summary>
    public class SettingsModel
    {
        public string Theme { get; set; } = "dark";
        public string ServerUrl { get; set; } = "http://127.0.0.1:8000";
        public bool VoiceEnabled { get; set; } = true;
        public bool NotificationsEnabled { get; set; } = true;
        public string WakeWord { get; set; } = "Hey CAI";
        public string PreferredModel { get; set; } = "phi-3-mini-onnx";
        public bool AutoStartOnBoot { get; set; } = false;

        /// <summary>List of model IDs that have been downloaded.</summary>
        public List<string> DownloadedModels { get; set; } = new List<string>();
    }
}
