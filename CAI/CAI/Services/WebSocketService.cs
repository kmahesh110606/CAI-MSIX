using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;

namespace CAI.Services
{
    /// <summary>
    /// WebSocket client for real-time communication with the Django Channels server.
    /// Handles task push and chat sync channels.
    /// </summary>
    public class WebSocketService : IDisposable
    {
        private ClientWebSocket? _taskSocket;
        private ClientWebSocket? _chatSocket;
        private CancellationTokenSource? _cts;
        private string _baseUrl = "ws://127.0.0.1:8000";

        public event EventHandler<string>? OnTaskReceived;
        public event EventHandler<string>? OnChatMessageReceived;
        public event EventHandler<string>? OnConnectionStatusChanged;

        public bool IsConnected => _taskSocket?.State == WebSocketState.Open;

        /// <summary>
        /// Connect to WebSocket channels for a specific device.
        /// </summary>
        public async Task ConnectAsync(Guid deviceId)
        {
            _cts = new CancellationTokenSource();

            var serverUrl = ApplicationData.Current.LocalSettings.Values["cai_server_url"] as string
                ?? "http://127.0.0.1:8000";
            _baseUrl = serverUrl.Replace("http://", "ws://").Replace("https://", "wss://");

            var token = ApplicationData.Current.LocalSettings.Values["cai_access_token"] as string;

            // Connect task channel
            _taskSocket = new ClientWebSocket();
            if (!string.IsNullOrEmpty(token))
            {
                _taskSocket.Options.SetRequestHeader("Authorization", $"Bearer {token}");
            }

            try
            {
                await _taskSocket.ConnectAsync(
                    new Uri($"{_baseUrl}/ws/tasks/{deviceId}/"),
                    _cts.Token);
                _ = ReceiveLoop(_taskSocket, "task", _cts.Token);
                OnConnectionStatusChanged?.Invoke(this, "Tasks: Connected");
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged?.Invoke(this, $"Tasks: Failed - {ex.Message}");
            }

            // Connect chat channel
            _chatSocket = new ClientWebSocket();
            if (!string.IsNullOrEmpty(token))
            {
                _chatSocket.Options.SetRequestHeader("Authorization", $"Bearer {token}");
            }

            try
            {
                await _chatSocket.ConnectAsync(
                    new Uri($"{_baseUrl}/ws/chats/{deviceId}/"),
                    _cts.Token);
                _ = ReceiveLoop(_chatSocket, "chat", _cts.Token);
                OnConnectionStatusChanged?.Invoke(this, "Chats: Connected");
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged?.Invoke(this, $"Chats: Failed - {ex.Message}");
            }
        }

        /// <summary>Send a message through the chat WebSocket.</summary>
        public async Task SendChatMessageAsync(object message)
        {
            if (_chatSocket?.State != WebSocketState.Open) return;

            var json = JsonConvert.SerializeObject(new
            {
                type = "chat_message",
                message,
            });

            var bytes = Encoding.UTF8.GetBytes(json);
            await _chatSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                _cts?.Token ?? CancellationToken.None);
        }

        /// <summary>Send a task status update through the task WebSocket.</summary>
        public async Task SendTaskUpdateAsync(string taskId, string status, string result = "", string error = "")
        {
            if (_taskSocket?.State != WebSocketState.Open) return;

            var json = JsonConvert.SerializeObject(new
            {
                type = "task_status_update",
                task_id = taskId,
                status,
                result,
                error_message = error,
            });

            var bytes = Encoding.UTF8.GetBytes(json);
            await _taskSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                _cts?.Token ?? CancellationToken.None);
        }

        /// <summary>Disconnect all WebSocket connections.</summary>
        public async Task DisconnectAsync()
        {
            _cts?.Cancel();

            if (_taskSocket?.State == WebSocketState.Open)
            {
                await _taskSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }

            if (_chatSocket?.State == WebSocketState.Open)
            {
                await _chatSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }

            OnConnectionStatusChanged?.Invoke(this, "Disconnected");
        }

        private async Task ReceiveLoop(ClientWebSocket socket, string channel, CancellationToken ct)
        {
            var buffer = new byte[4096];

            while (!ct.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        OnConnectionStatusChanged?.Invoke(this, $"{channel}: Disconnected");
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    if (channel == "task")
                        OnTaskReceived?.Invoke(this, message);
                    else
                        OnChatMessageReceived?.Invoke(this, message);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    OnConnectionStatusChanged?.Invoke(this, $"{channel}: Error");
                    break;
                }
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _taskSocket?.Dispose();
            _chatSocket?.Dispose();
            _cts?.Dispose();
        }
    }
}
