namespace VocabularyRepositoryFromPDF.WebSocket;

using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

public class WebSocketManagerTests
{
    [Fact]
    public async Task AddSocketAsync_ShouldAddWebSocketToConnections()
    {
        // Arrange
        var manager = new WebSocketManager();
        var mockSocket = new Mock<WebSocket>();
        // Act
        await manager.AddSocketAsync(mockSocket.Object);
        // Assert
        Assert.Single(manager.Sockets);
    }

    [Fact]
    public async Task BroadcastMessageAsync_ShouldSendMessageToAllConnectedSockets()
    {
        // Arrange
        var manager = new WebSocketManager();

        var mockSocket1 = new Mock<WebSocket>();
        var mockSocket2 = new Mock<WebSocket>();

        // Simulate WebSocket states
        mockSocket1.Setup(s => s.State).Returns(WebSocketState.Open);
        mockSocket2.Setup(s => s.State).Returns(WebSocketState.Open);

        // Add mock sockets to the manager
        await manager.AddSocketAsync(mockSocket1.Object);
        await manager.AddSocketAsync(mockSocket2.Object);

        // Act
        await manager.BroadcastMessageAsync("Hello, WebSockets!");

        // Assert
        mockSocket1.Verify(socket => socket.SendAsync(
                It.IsAny<ArraySegment<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);

        mockSocket2.Verify(socket => socket.SendAsync(
                It.IsAny<ArraySegment<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveSocketAsync_ShouldRemoveClosedSockets()
    {
        // Arrange
        var manager = new WebSocketManager();
        var mockSocket = new Mock<WebSocket>();
        mockSocket.Setup(s => s.State).Returns(WebSocketState.Closed);
        await manager.AddSocketAsync(mockSocket.Object);

        // Assert
        Assert.Empty(manager.Sockets);
    }

    [Fact]
    public async Task GetSockets_ShouldReturnAllManagedSockets()
    {
        // Arrange
        var manager = new WebSocketManager();

        var mockSocket1 = new Mock<WebSocket>();
        var mockSocket2 = new Mock<WebSocket>();
        
        await manager.AddSocketAsync(mockSocket1.Object);
        await manager.AddSocketAsync(mockSocket2.Object);
        // Act
        var sockets = manager.Sockets;
        // Assert
        Assert.Equal(2, sockets.Count);
    }
}