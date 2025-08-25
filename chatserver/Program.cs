using ChatServer.Data;
using ChatServer.Services;
using ChatServer.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ChatServer.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load .env file
Env.Load();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ SignalR Services
builder.Services.AddSignalR();

// ✅ Entity Framework Configuration
var connectionString = $"Host=localhost;Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};Password={Environment.GetEnvironmentVariable("POSTGRES_PW")};Port={Environment.GetEnvironmentVariable("DB_PORT")}";

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseNpgsql(connectionString));

// ✅ Services Registration
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPresenceService, PresenceService>();

// ✅ CORS für SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:3000", "http://localhost:5173", "https://localhost:7070")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials(); // Wichtig für SignalR
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}

// ✅ Static Files für Test UI
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthorization();

// ✅ Map Controllers und SignalR Hub
app.MapControllers();
app.MapHub<ChatHub>("/chathub");

// ✅ Auto-migrate und Seed Data
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        try
        {
            context.Database.Migrate();
            await SeedDefaultData(context);
            Console.WriteLine("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database migration failed: {ex.Message}");
        }
    }
}

app.Run();

// ✅ Seed Default Data
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
                Description = "General discussion room",
                CreatedAt = DateTime.UtcNow
            };

            context.ChatRooms.Add(generalRoom);
            await context.SaveChangesAsync();
            Console.WriteLine("Default 'General' chat room created.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding data: {ex.Message}");
    }
}