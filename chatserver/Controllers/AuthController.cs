using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ChatServer.Services;
using ChatServer.Models.DTOs;
using ChatServer.Models;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService; // Dependency Injection
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, IPasswordHasher<User> passwordHasher, IJwtService jwtService)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            try
            {

                if (await _userService.UsernameExistsAsync(registerDto.Username))
                {
                    return BadRequest(new { Error = "Username already exists", Field = "Username" });
                }

                if (await _userService.EmailExistsAsync(registerDto.Email))
                {
                    return BadRequest(new { Error = "Email already exists", Field = "Email" });
                }
                var user = new User
                {
                    Username = registerDto.Username,
                    Firstname = registerDto.Firstname,
                    Lastname = registerDto.Lastname,
                    Email = registerDto.Email,
                    Password_Hash = string.Empty
                };

                user.Password_Hash = _passwordHasher.HashPassword(user, registerDto.Password);

                var createdUser = await _userService.CreateUserAsync(user);

                var token = _jwtService.GenerateToken(createdUser);


                var response = new AuthResponseDto
                {
                    Username = createdUser.Username,
                    UserId = createdUser.Id,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(1) // Example expiration time
                };
                return CreatedAtAction(nameof(GetCurrentUser), new { id = createdUser.Id }, response);
            }
            // ✅ Specific Exception Handling
            catch (ArgumentException ex)
            {
                // Validation errors from UserService
                return BadRequest(new { Error = ex.Message, Type = "ValidationError" });
            }
            catch (InvalidOperationException ex)
            {
                // Business logic errors (username exists, etc.)
                return Conflict(new { Error = ex.Message, Type = "BusinessLogicError" });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Security/Auth errors
                return Unauthorized(new { Error = ex.Message, Type = "SecurityError" });
            }
            catch (TimeoutException ex)
            {
                // Database timeout
                return StatusCode(408, new { Error = "Request timeout", Details = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { 
                    Error = "An unexpected error occurred", 
                    RequestId = HttpContext.TraceIdentifier 
                });
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var Username = loginDto.Username;
            var password = loginDto.Password;
            try
            {
                // User anhand des Benutzernamens abrufen
                var user = await _userService.GetUserByUsernameAsync(Username);

                if (user == null)
                {
                    return Unauthorized(new { Error = "Invalid credentials" });
                }

                // Passwort-Hashing überprüfen
                var result = _passwordHasher.VerifyHashedPassword(user, user.Password_Hash, password);

                if (result == PasswordVerificationResult.Failed)
                {
                    return Unauthorized(new { Error = "Invalid credentials" });
                }

                // Bei Success oder SuccessRehashNeeded
                if (result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    // Optional: Passwort neu hashen für bessere Sicherheit
                    user.Password_Hash = _passwordHasher.HashPassword(user, loginDto.Password);
                    await _userService.UpdateUserAsync(user.Id, user);
                }

                var token = _jwtService.GenerateToken(user);

                // Response
                var response = new AuthResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                };

                return Ok(new
                {
                    Message = "Login successful",
                    Data = response,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message, Type = "ValidationError" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Error = ex.Message, Type = "AuthenticationError" });
            }
            catch (TimeoutException ex)
            {
                return StatusCode(408, new { Error = "Request timeout", Details = ex.Message });
            }
            catch (Exception)
            {
                // _logger.LogError(ex, "Unexpected error during login");
                return StatusCode(500, new { 
                    Error = "An unexpected error occurred", 
                    RequestId = HttpContext.TraceIdentifier 
                });
            }
        }
        
        [HttpGet("me")]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            // TODO: Später mit JWT implementieren
            return Ok();
        }
    }
}