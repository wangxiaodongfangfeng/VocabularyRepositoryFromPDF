using System.Net.WebSockets;
using System.Text;

namespace VocabularyRepositoryFromPDF.WebSocket;

/// <summary>
/// 
/// </summary>
public class WebSocketManager : IWebSocketManager
{
    private readonly List<System.Net.WebSockets.WebSocket> _sockets = [];


    public List<System.Net.WebSockets.WebSocket> Sockets => _sockets;

    /// <summary>
    /// queue the sockets when connected event happened
    /// </summary>
    /// <param name="socket"></param>
    public async Task AddSocketAsync(System.Net.WebSockets.WebSocket socket)
    {
        _sockets.Add(socket);
        var buffer = new byte[1024 * 4];

        // Listen for incoming messages (optional)
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (!result.CloseStatus.HasValue) continue;
            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _sockets.Remove(socket);
        }
    }

    /// <summary>
    /// Send the event to the client
    /// </summary>
    /// <param name="message"></param>
    public async Task BroadcastMessageAsync(string message)
    {
        var messageBuffer = Encoding.UTF8.GetBytes(message);

        foreach (var socket in _sockets.ToList()) // Use ToList to avoid collection modification errors
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
            {
                _sockets.Remove(socket);
            }
        }
    }
}