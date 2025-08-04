namespace ChatServer.Services
{ 
    using ChatServer.Models;
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> UpdateUserAsync(int userId, User user);
        Task DeleteUserAsync(int userId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> UsernameExistsAsync(object username);
    }

}

