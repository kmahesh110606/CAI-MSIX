using System;
using Microsoft.UI.Xaml.Controls;

namespace CAI.Utils
{
    /// <summary>
    /// Navigation helper service for frame-based page navigation.
    /// </summary>
    public class NavigationHelper
    {
        private Frame? _frame;

        /// <summary>Set the navigation frame.</summary>
        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        /// <summary>Navigate to a page type.</summary>
        public bool NavigateTo(Type pageType, object? parameter = null)
        {
            if (_frame == null) return false;
            return _frame.Navigate(pageType, parameter);
        }

        /// <summary>Go back if possible.</summary>
        public void GoBack()
        {
            if (_frame?.CanGoBack == true)
                _frame.GoBack();
        }

        /// <summary>Whether going back is possible.</summary>
        public bool CanGoBack => _frame?.CanGoBack == true;
    }
}
