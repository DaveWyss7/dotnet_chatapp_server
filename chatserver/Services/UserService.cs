using Microsoft.EntityFrameworkCore;
using ChatServer.Data;
using ChatServer.Models;

namespace ChatServer.Services
{
    public class UserService : IUserService
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ChatDbContext context, ILogger<UserService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username cannot be null or empty", nameof(user));

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email cannot be null or empty", nameof(user));

            try
            {
                // ✅ Check if username already exists
                if (await UsernameExistsAsync(user.Username))
                {
                    _logger.LogWarning("Attempt to create user with existing username: {Username}", user.Username);
                    throw new InvalidOperationException($"Username '{user.Username}' already exists");
                }

                // ✅ Check if email already exists
                if (await EmailExistsAsync(user.Email))
                {
                    _logger.LogWarning("Attempt to create user with existing email: {Email}", user.Email);
                    throw new InvalidOperationException($"Email '{user.Email}' already exists");
                }

                // ✅ Add user to database context
                _context.Users.Add(user);
                
                // ✅ Save changes to database
                await _context.SaveChangesAsync();

                _logger.LogInformation("User created successfully: {Username} with ID {UserId}", 
                    user.Username, user.Id);

                return user;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating user {Username}", user.Username);
                throw new InvalidOperationException("Database error occurred while creating user", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating user {Username}", user.Username);
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID provided: {UserId}", id);
                return null;
            }

            try
            {
                var user = await _context.Users.FindAsync(id);
                
                if (user == null)
                {
                    _logger.LogDebug("User not found with ID: {UserId}", id);
                }
                else
                {
                    _logger.LogDebug("User found: {Username} (ID: {UserId})", user.Username, user.Id);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by ID: {UserId}", id);
                throw new InvalidOperationException("Error retrieving user", ex);
            }
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("Empty username provided for lookup");
                return null;
            }

            try
            {
                // ✅ Case-insensitive username search
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

                if (user == null)
                {
                    _logger.LogDebug("User not found with username: {Username}", username);
                }
                else
                {
                    _logger.LogDebug("User found: {Username} (ID: {UserId})", user.Username, user.Id);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by username: {Username}", username);
                throw new InvalidOperationException("Error retrieving user", ex);
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            try
            {
                // ✅ Case-insensitive check for better UX
                var exists = await _context.Users
                    .AnyAsync(u => u.Username.ToLower() == username.ToLower());

                _logger.LogDebug("Username existence check for '{Username}': {Exists}", username, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username existence: {Username}", username);
                throw new InvalidOperationException("Error checking username existence", ex);
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // ✅ Case-insensitive email check
                var exists = await _context.Users
                    .AnyAsync(u => u.Email.ToLower() == email.ToLower());

                _logger.LogDebug("Email existence check for '{Email}': {Exists}", email, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email existence: {Email}", email);
                throw new InvalidOperationException("Error checking email existence", ex);
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .OrderBy(u => u.Username)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {UserCount} users from database", users.Count);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw new InvalidOperationException("Error retrieving users", ex);
            }
        }

        public async Task<User> UpdateUserAsync(int userId, User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            try
            {
                // ✅ Find existing user
                var existingUser = await _context.Users.FindAsync(userId);
                if (existingUser == null)
                {
                    _logger.LogWarning("Attempt to update non-existent user with ID: {UserId}", userId);
                    throw new ArgumentException($"User with ID {userId} not found");
                }

                // ✅ Check if new username is already taken (by another user)
                if (!string.Equals(existingUser.Username, user.Username, StringComparison.OrdinalIgnoreCase))
                {
                    var usernameExists = await _context.Users
                        .AnyAsync(u => u.Id != userId && u.Username.ToLower() == user.Username.ToLower());
                    
                    if (usernameExists)
                        throw new InvalidOperationException($"Username '{user.Username}' is already taken");
                }

                // ✅ Check if new email is already taken (by another user)
                if (!string.Equals(existingUser.Email, user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailExists = await _context.Users
                        .AnyAsync(u => u.Id != userId && u.Email.ToLower() == user.Email.ToLower());
                    
                    if (emailExists)
                        throw new InvalidOperationException($"Email '{user.Email}' is already taken");
                }

                // ✅ Update user properties
                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                existingUser.Firstname = user.Firstname;
                existingUser.Lastname = user.Lastname;
                
                // ✅ Only update password hash if provided
                if (!string.IsNullOrWhiteSpace(user.Password_Hash))
                {
                    existingUser.Password_Hash = user.Password_Hash;
                }

                // ✅ Save changes
                await _context.SaveChangesAsync();

                _logger.LogInformation("User updated successfully: {Username} (ID: {UserId})", 
                    existingUser.Username, existingUser.Id);

                return existingUser;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating user {UserId}", userId);
                throw new InvalidOperationException("Database error occurred while updating user", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating user {UserId}", userId);
                throw;
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Invalid user ID provided for deletion: {UserId}", userId);
                throw new ArgumentException("Invalid user ID", nameof(userId));
            }

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Attempt to delete non-existent user with ID: {UserId}", userId);
                    return; // Silently ignore if user doesn't exist
                }

                // ✅ Remove user from context
                _context.Users.Remove(user);
                
                // ✅ Save changes
                await _context.SaveChangesAsync();

                _logger.LogInformation("User deleted successfully: {Username} (ID: {UserId})", 
                    user.Username, user.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting user {UserId}", userId);
                throw new InvalidOperationException("Database error occurred while deleting user", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting user {UserId}", userId);
                throw;
            }
        }

        // ✅ Dispose pattern for proper resource cleanup
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}