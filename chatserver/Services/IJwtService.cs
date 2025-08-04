namespace ChatServer.Services
{
    using ChatServer.Models;
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}