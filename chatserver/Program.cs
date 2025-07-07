using ChatServer.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// CORS policy
// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(builder =>
//     {
//         builder.SetIsOriginAllowed(_ => true)  // Allow any origin
//                .AllowAnyMethod()
//                .AllowAnyHeader()
//                .AllowCredentials();  // Required for WebSocket
//     });
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection();
// app.UseCors();

// Configure WebSocket
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);
app.UseMiddleware<WebSocketMiddleware>();

// app.UseAuthorization();
// app.MapControllers();

app.Run();
