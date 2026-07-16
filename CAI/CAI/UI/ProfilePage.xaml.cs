using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CAI.UI
{
    /// <summary>
    /// Profile page showing user information and sign out option.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            LoadProfile();
        }

        private void LoadProfile()
        {
            var user = App.Auth.CurrentUser;
            if (user != null)
            {
                UserNameText.Text = user.DisplayName;
                UserEmailText.Text = user.Email;
                JoinedDateText.Text = user.DateJoined?.ToString("MMMM yyyy") ?? "—";
            }

            // Show installed models
            var models = App.Config.Settings.DownloadedModels;
            if (models != null && models.Count > 0)
            {
                InstalledModelsText.Text = string.Join(", ", models);
            }
            else
            {
                InstalledModelsText.Text = "None";
            }
        }

        private async void SignOut_Click(object sender, RoutedEventArgs e)
        {
            await App.Auth.LogoutAsync();
            App.NavigateToLogin();
        }
    }
}
