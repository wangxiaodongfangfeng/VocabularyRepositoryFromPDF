using System.Net.WebSockets;
using System.Text;
using WebSocketManager = VocabularyRepositoryFromPDF.WebSocket.WebSocketManager;

var builder = WebApplication.CreateBuilder(args);
const string corsPolicy = "AllowWestWorldRequest";
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddEnvironmentVariables();
var origin = builder.Configuration["AllowedCorsOrigin"] ?? "https://www.vocabulary.com/*";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy,
        policy => { policy.WithOrigins(origin.Split(',')).AllowAnyMethod().AllowAnyHeader().AllowCredentials(); });
});
builder.Services.AddSingleton<WebSocketManager>();
builder.Services.AddControllers();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    Console.WriteLine($"CrosSetting is got which is {origin}");
}
app.UseCors(corsPolicy);
// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
// Enable WebSocket support
app.UseWebSockets();
app.Map("/ws", async context =>
{
    var webSocketManager = app.Services.GetService<WebSocketManager>();
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await webSocketManager?.AddSocketAsync(socket)!;
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
}).RequireCors(corsPolicy);
app.MapControllers().RequireCors(corsPolicy).WithOpenApi();
app.Run();

