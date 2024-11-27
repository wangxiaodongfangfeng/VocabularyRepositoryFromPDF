using Microsoft.AspNetCore.Mvc;
using WebSocketManager = VocabularyRepositoryFromPDF.WebSocket.WebSocketManager;

namespace VocabularyRepositoryFromPDF.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class WordsController(WebSocketManager webSocketManager) : ControllerBase
{
    [HttpPost("")]
    public IActionResult Save([FromBody] WordInfo request)
    {
        if (string.IsNullOrWhiteSpace(request.Word))
        {
            return BadRequest("Word cannot be empty.");
        }

        // Broadcast the word to WebSocket clients
        return Ok(new { Message = "Word sent to clients.", Word = request.Word });
    }

    [HttpPost("")]
    public async Task<IActionResult> Lookup([FromBody] WordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Word))
        {
            return BadRequest("Word cannot be empty.");
        }

        // Broadcast the word to WebSocket clients
        await webSocketManager.BroadcastMessageAsync(request.Word);
        return Ok(new { Message = "Word sent to clients.", Word = request.Word });
    }

    public record WordInfo(string? Word, string? Description, string? Url)
    {
        public override string ToString()
        {
            return @$"{Word} at ``{Url}`` and {Description}";
        }
    }

    public class WordRequest
    {
        public string Word { get; set; }
    }
}