using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CAI.UI
{
    /// <summary>
    /// Redesigned onboarding page with 5 feature slides and Back/Next navigation.
    /// </summary>
    public sealed partial class OnboardingPage : Page
    {
        private const int TotalSlides = 5;

        public OnboardingPage()
        {
            InitializeComponent();
            UpdateNavigationState();
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageIndicator == null) return;
            PageIndicator.SelectedPageIndex = OnboardingFlipView.SelectedIndex;
            UpdateNavigationState();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (OnboardingFlipView.SelectedIndex > 0)
            {
                OnboardingFlipView.SelectedIndex--;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (OnboardingFlipView.SelectedIndex < TotalSlides - 1)
            {
                // Go to next slide
                OnboardingFlipView.SelectedIndex++;
            }
            else
            {
                // Last slide — complete onboarding and go to login
                App.Config.CompleteFirstLaunch();
                App.NavigateToLogin();
            }
        }

        private void UpdateNavigationState()
        {
            if (BackButton == null || NextButtonText == null || NextButtonIcon == null) return;

            var index = OnboardingFlipView.SelectedIndex;

            // Show/hide back button
            BackButton.Visibility = index > 0 ? Visibility.Visible : Visibility.Collapsed;

            // Update next button text on last slide
            if (index >= TotalSlides - 1)
            {
                NextButtonText.Text = "GET STARTED";
                NextButtonIcon.Glyph = "\uE76C"; // right arrow
            }
            else
            {
                NextButtonText.Text = "NEXT";
                NextButtonIcon.Glyph = "\uE76C"; // right arrow
            }
        }
    }
}
