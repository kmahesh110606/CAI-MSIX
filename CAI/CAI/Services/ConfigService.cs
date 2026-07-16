using Newtonsoft.Json;
using Windows.Storage;
using CAI.Models;

namespace CAI.Services
{
    /// <summary>
    /// Manages application settings persistence using Windows LocalSettings.
    /// </summary>
    public class ConfigService
    {
        private const string SettingsKey = "cai_app_settings";
        private const string FirstLaunchKey = "cai_first_launch_complete";
        private const string ModelSetupKey = "cai_model_setup_complete";

        private SettingsModel _settings;

        public ConfigService()
        {
            _settings = LoadSettings();
        }

        /// <summary>Current application settings.</summary>
        public SettingsModel Settings => _settings;

        /// <summary>Whether this is the first launch of the app.</summary>
        public bool IsFirstLaunch
        {
            get
            {
                var settings = ApplicationData.Current.LocalSettings;
                return settings.Values[FirstLaunchKey] as string != "true";
            }
        }

        /// <summary>Whether the model setup has been completed.</summary>
        public bool IsModelSetupComplete
        {
            get
            {
                var settings = ApplicationData.Current.LocalSettings;
                return settings.Values[ModelSetupKey] as string == "true";
            }
        }

        /// <summary>Mark the first launch as complete.</summary>
        public void CompleteFirstLaunch()
        {
            ApplicationData.Current.LocalSettings.Values[FirstLaunchKey] = "true";
        }

        /// <summary>Mark model setup as complete.</summary>
        public void CompleteModelSetup()
        {
            ApplicationData.Current.LocalSettings.Values[ModelSetupKey] = "true";
        }

        /// <summary>Save current settings to local storage.</summary>
        public void Save()
        {
            var json = JsonConvert.SerializeObject(_settings);
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = json;
        }

        /// <summary>Update and persist a specific setting.</summary>
        public void UpdateSetting(System.Action<SettingsModel> update)
        {
            update(_settings);
            Save();
        }

        /// <summary>Reset all settings to defaults.</summary>
        public void ResetToDefaults()
        {
            _settings = new SettingsModel();
            Save();
        }

        private SettingsModel LoadSettings()
        {
            var json = ApplicationData.Current.LocalSettings.Values[SettingsKey] as string;
            if (!string.IsNullOrEmpty(json))
            {
                return JsonConvert.DeserializeObject<SettingsModel>(json)
                    ?? new SettingsModel();
            }
            return new SettingsModel();
        }
    }
}
