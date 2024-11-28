using System.Net.WebSockets;
using System.Text;

namespace VocabularyRepositoryFromPDF.WebSocket;

/// <summary>
/// 
/// </summary>
public class WebSocketManager : IWebSocketManager, IDisposable
{
    public List<(System.Net.WebSockets.WebSocket socket, CancellationToken cancellationToken)> Sockets { get; } = [];

    /// <summary>
    /// queue the sockets when connected event happened
    /// </summary>
    /// <param name="socket"></param>
    public async Task AddSocketAsync(System.Net.WebSockets.WebSocket socket)
    {
        var socketPair = (socket, CancellationToken.None);
        Sockets.Add(socketPair);
        var buffer = new byte[1024 * 4];

        // Listen for incoming messages (optional)
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (!result.CloseStatus.HasValue) continue;
            Sockets.Remove(socketPair);
        }
    }

    /// <summary>
    /// Send the event to the client
    /// </summary>
    /// <param name="message"></param>
    public async Task BroadcastMessageAsync(string message)
    {
        var messageBuffer = Encoding.UTF8.GetBytes(message);

        foreach (var socketPair in Sockets.ToList()) // Use ToList to avoid collection modification errors
        {
            if (socketPair.socket.State == WebSocketState.Open)
            {
                await socketPair.socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text,
                    true,
                    socketPair.cancellationToken);
            }
            else
            {
                Sockets.Remove(socketPair);
            }
        }
    }

    public void Dispose() =>
        Sockets.ForEach(s =>
        {
            if (s.socket.State == WebSocketState.Open)
            {
                s.socket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
            }
        });
}