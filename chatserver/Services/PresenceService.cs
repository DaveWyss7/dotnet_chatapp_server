using Microsoft.EntityFrameworkCore;
using ChatServer.Data;
using ChatServer.Models;

namespace ChatServer.Services
{
    public class PresenceService : IPresenceService
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<PresenceService> _logger;

        public PresenceService(ChatDbContext context, ILogger<PresenceService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ConnectUserAsync(int userId, string connectionId)
        {
            try
            {
                var userSession = new UserSession
                {
                    UserId = userId,
                    ConnectionId = connectionId,
                    ConnectedAt = DateTime.UtcNow,
                    IsOnline = true
                };

                _context.UserSessions.Add(userSession);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", 
                    userId, connectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting user {UserId}", userId);
                throw;
            }
        }

        public async Task DisconnectUserAsync(string connectionId)
        {
            try
            {
                var userSession = await _context.UserSessions
                    .FirstOrDefaultAsync(us => us.ConnectionId == connectionId && us.IsOnline);

                if (userSession != null)
                {
                    userSession.IsOnline = false;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("User {UserId} disconnected", userSession.UserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disconnecting connection {ConnectionId}", connectionId);
            }
        }

        public async Task<IEnumerable<User>> GetOnlineUsersAsync()
        {
            try
            {
                return await _context.UserSessions
                    .Where(us => us.IsOnline)
                    .Include(us => us.User)
                    .Select(us => us.User)
                    .Distinct()
                    .OrderBy(u => u.Username)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving online users");
                throw;
            }
        }
    }
}