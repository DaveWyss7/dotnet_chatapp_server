using ChatServer.Models;

namespace ChatServer.Services
{
    public interface IPresenceService
    {
        Task ConnectUserAsync(int userId, string connectionId);
        Task DisconnectUserAsync(string connectionId);
        Task<IEnumerable<User>> GetOnlineUsersAsync();
    }
}