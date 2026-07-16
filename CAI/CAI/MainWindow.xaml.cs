using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CAI
{
    /// <summary>
    /// Main application window with expanded navigation sidebar.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set title bar
            Title = "CAI — Your AI Assistant";
            ExtendsContentIntoTitleBar = true;

            // Navigate to chat page by default
            ContentFrame.Navigate(typeof(UI.ChatPage));
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(UI.SettingsPage));
                return;
            }

            if (args.SelectedItemContainer is NavigationViewItem item)
            {
                var tag = item.Tag?.ToString();
                switch (tag)
                {
                    case "chat":
                        ContentFrame.Navigate(typeof(UI.ChatPage));
                        break;
                    case "chat-history":
                        ContentFrame.Navigate(typeof(UI.ChatHistoryPage));
                        break;
                    case "task-history":
                        ContentFrame.Navigate(typeof(UI.TaskHistoryPage));
                        break;
                    case "devices":
                        ContentFrame.Navigate(typeof(UI.SettingsPage));
                        break;
                    case "profile":
                        ContentFrame.Navigate(typeof(UI.ProfilePage));
                        break;
                }
            }
        }
    }
}
