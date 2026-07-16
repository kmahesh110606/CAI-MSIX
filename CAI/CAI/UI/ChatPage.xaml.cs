using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using CAI.Models;

namespace CAI.UI
{
    public sealed partial class ChatPage : Page
    {
        private readonly ObservableCollection<ChatMessageModel> _messages = new();

        public ChatPage()
        {
            InitializeComponent();
            MessagesList.ItemsSource = _messages;

            // Add welcome message
            _messages.Add(new ChatMessageModel
            {
                Id = Guid.NewGuid(),
                Role = "assistant",
                Content = "Hello! I'm CAI, your AI assistant. How can I help you today?",
                CreatedAt = DateTime.Now,
            });
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void MessageInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private void SendMessage()
        {
            var text = MessageInput.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            // Add user message
            _messages.Add(new ChatMessageModel
            {
                Id = Guid.NewGuid(),
                Role = "user",
                Content = text,
                CreatedAt = DateTime.Now,
            });

            MessageInput.Text = string.Empty;

            // Scroll to bottom
            if (_messages.Count > 0)
            {
                MessagesList.ScrollIntoView(_messages[_messages.Count - 1]);
            }

            // TODO: Send to API and get AI response
            // For now, echo a placeholder response
            _messages.Add(new ChatMessageModel
            {
                Id = Guid.NewGuid(),
                Role = "assistant",
                Content = $"I received: \"{text}\"\n\n(AI inference will be connected in a future update)",
                CreatedAt = DateTime.Now,
            });

            if (_messages.Count > 0)
            {
                MessagesList.ScrollIntoView(_messages[_messages.Count - 1]);
            }
        }

        private void Voice_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Trigger SpeechService
            _messages.Add(new ChatMessageModel
            {
                Id = Guid.NewGuid(),
                Role = "system",
                Content = "🎤 Voice input is not yet connected. Integration coming soon.",
                CreatedAt = DateTime.Now,
            });
        }
    }
}
