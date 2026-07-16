using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CAI.UI
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = App.Config.Settings;
            var user = App.Auth.CurrentUser;

            // Account
            if (user != null)
            {
                UserEmailText.Text = user.Email;
                UserNameText.Text = user.DisplayName;
            }

            // Server
            ServerUrlBox.Text = settings.ServerUrl;

            // Model
            foreach (ComboBoxItem item in ModelSelector.Items)
            {
                if (item.Tag?.ToString() == settings.PreferredModel)
                {
                    ModelSelector.SelectedItem = item;
                    break;
                }
            }

            // Preferences
            VoiceToggle.IsOn = settings.VoiceEnabled;
            NotifToggle.IsOn = settings.NotificationsEnabled;
            AutoStartToggle.IsOn = settings.AutoStartOnBoot;
        }

        private async void SignOut_Click(object sender, RoutedEventArgs e)
        {
            await App.Auth.LogoutAsync();
            App.NavigateToLogin();
        }

        private void SaveServerUrl_Click(object sender, RoutedEventArgs e)
        {
            var url = ServerUrlBox.Text.Trim();
            if (!string.IsNullOrEmpty(url))
            {
                App.Config.UpdateSetting(s => s.ServerUrl = url);
                App.Api.SetBaseUrl(url);
            }
        }

        private void SaveModel_Click(object sender, RoutedEventArgs e)
        {
            if (ModelSelector.SelectedItem is ComboBoxItem item)
            {
                App.Config.UpdateSetting(s => s.PreferredModel = item.Tag?.ToString() ?? "both");
            }
        }

        private void VoiceToggle_Toggled(object sender, RoutedEventArgs e)
        {
            App.Config.UpdateSetting(s => s.VoiceEnabled = VoiceToggle.IsOn);
        }

        private void NotifToggle_Toggled(object sender, RoutedEventArgs e)
        {
            App.Config.UpdateSetting(s => s.NotificationsEnabled = NotifToggle.IsOn);
        }

        private void AutoStartToggle_Toggled(object sender, RoutedEventArgs e)
        {
            App.Config.UpdateSetting(s => s.AutoStartOnBoot = AutoStartToggle.IsOn);
        }
    }
}
