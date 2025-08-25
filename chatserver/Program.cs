using ChatServer.Data;
using ChatServer.Services;
using ChatServer.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ChatServer.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load .env file (nur wenn nicht in Docker)
if (File.Exists(".env"))
{
    Env.Load();
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ SignalR Services
builder.Services.AddSignalR();

// ✅ Build Connection String (Docker + Local kompatibel)
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5433";
var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "chatapp";
var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "sysadmin";
var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PW") ?? "password";

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

Console.WriteLine($"🔗 Connection String: Host={dbHost}, Port={dbPort}, Database={dbName}, User={dbUser}");

// ✅ Entity Framework Configuration
builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseNpgsql(connectionString));

// ✅ Services Registration
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPresenceService, PresenceService>();

// ✅ CORS for Docker + Local
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:3000",     // Docker Frontend
                    "http://localhost:5173",     // Vite Dev
                    "http://chat-ui",            // Docker Internal
                    "https://localhost:7070"     // Local HTTPS
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();

// ✅ Map Controllers and SignalR Hub
app.MapControllers();
app.MapHub<ChatHub>("/chathub");

// ✅ Auto-migrate and seed data
await InitializeDatabase(app);

Console.WriteLine("🚀 Chat Server started successfully!");
Console.WriteLine($"📡 SignalR Hub available at: /chathub");
Console.WriteLine($"📊 Swagger UI: {(app.Environment.IsDevelopment() ? "/swagger" : "disabled")}");

app.Run();

// ✅ Database Initialization
static async Task InitializeDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    
    try
    {
        Console.WriteLine("🔄 Applying database migrations...");
        await context.Database.MigrateAsync();
        
        Console.WriteLine("🌱 Seeding default data...");
        await SeedDefaultData(context);
        
        Console.WriteLine("✅ Database initialized successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
        Console.WriteLine($"🔍 Stack trace: {ex.StackTrace}");
    }
}

static async Task SeedDefaultData(ChatDbContext context)
{
    try
    {
        // Create default "General" chat room
        if (!await context.ChatRooms.AnyAsync(cr => cr.Name == "General"))
        {
            var generalRoom = new ChatRoom
            {
                Name = "General",
                Description = "General discussion room for everyone",
                CreatedAt = DateTime.UtcNow
            };

            context.ChatRooms.Add(generalRoom);
            await context.SaveChangesAsync();
            Console.WriteLine("📝 Default 'General' chat room created.");
        }
        else
        {
            Console.WriteLine("📝 Default chat room already exists.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error seeding data: {ex.Message}");
    }
}