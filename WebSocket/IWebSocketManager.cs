namespace VocabularyRepositoryFromPDF.WebSocket;

public interface IWebSocketManager
{
    public Task AddSocketAsync(System.Net.WebSockets.WebSocket webSocket);
    public Task BroadcastMessageAsync(string message);
}