namespace ChatServer.Services
{
    using ChatServer.Models;
    public class UserService : IUserService
    {
        public async Task<User> CreateUserAsync(User user)
        {
            // Implement logic to create a user
            if (await UsernameExistsAsync(user.UserName))
            {
                throw new Exception("Username already exists");
            }
            ;
            if (await EmailExistsAsync(user.Email))
            {
                throw new Exception("Email already exists");
            }
            throw new NotImplementedException();

            user.Id = ""
            _user.Add(user);
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            await Task.Delay(50);
            // Implement logic to get a user by ID
            throw new NotImplementedException();
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            await Task.Delay(50);
            throw new NotImplementedException();
        }

        public async Task<User> UpdateUserAsync(int userId, User user)
        {
            await Task.Delay(50);
            // Implement logic to update a user
            throw new NotImplementedException();
        }

        public async Task DeleteUserAsync(int userId)
        {
            await Task.Delay(50);
            // Implement logic to delete a user
            throw new NotImplementedException();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            await Task.Delay(50);
            throw new NotImplementedException();

        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            await Task.Delay(50);
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            await Task.Delay(50);
            // Implement logic to get all users
            throw new NotImplementedException();
        }

    }

}