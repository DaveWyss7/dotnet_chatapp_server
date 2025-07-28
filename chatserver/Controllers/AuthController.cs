using Microsoft.AspNetCore.Mvc;
using ChatServer.Services;
using ChatServer.Models;

namespace ChatServer.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService; // Dependency Injection

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // Register & Login Endpoints that use UserService
        // Example: [HttpPost("register")]
        // public async Task<IActionResult> Register(User user) { ... }
    }
}