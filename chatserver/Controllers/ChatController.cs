using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ChatServer.Services;
using ChatServer.Models.DTOs;
using ChatServer.Models;
using ChatServer.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IPresenceService _presenceService;
        private readonly ChatDbContext _context;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            IMessageService messageService,
            IPresenceService presenceService,
            ChatDbContext context,
            ILogger<ChatController> logger)
        {
            _messageService = messageService;
            _presenceService = presenceService;
            _context = context;
            _logger = logger;
        }

        // ✅ Get Chat Rooms (Simple - alle Rooms)
        [HttpGet("rooms")]
        public async Task<ActionResult<IEnumerable<ChatRoom>>> GetChatRooms()
        {
            try
            {
                var rooms = await _context.ChatRooms
                    .OrderBy(r => r.Name)
                    .ToListAsync();
                
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat rooms");
                return StatusCode(500, new { Error = "An unexpected error occurred" });
            }
        }

        // ✅ Create Chat Room (Simple)
        [HttpPost("rooms")]
        public async Task<ActionResult<ChatRoom>> CreateChatRoom(CreateChatRoomDto createRoomDto)
        {
            try
            {
                // Check if room exists
                var existingRoom = await _context.ChatRooms
                    .FirstOrDefaultAsync(r => r.Name.ToLower() == createRoomDto.Name.ToLower());

                if (existingRoom != null)
                    return BadRequest(new { Error = "Room already exists" });

                var chatRoom = new ChatRoom
                {
                    Name = createRoomDto.Name,
                    Description = createRoomDto.Description,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ChatRooms.Add(chatRoom);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetChatRooms), new { id = chatRoom.Id }, chatRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chat room");
                return StatusCode(500, new { Error = "An unexpected error occurred" });
            }
        }

        // ✅ Get Messages for Room
        [HttpGet("rooms/{id}/messages")]
        public async Task<ActionResult<IEnumerable<MessageResponseDto>>> GetMessages(int id, [FromQuery] int limit = 50)
        {
            try
            {
                var messages = await _messageService.GetMessagesForRoomAsync(id, limit);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages for room {ChatRoomId}", id);
                return StatusCode(500, new { Error = "An unexpected error occurred" });
            }
        }

        // ✅ Get Online Users
        [HttpGet("online-users")]
        public async Task<ActionResult> GetOnlineUsers()
        {
            try
            {
                var onlineUsers = await _presenceService.GetOnlineUsersAsync();
                var userList = onlineUsers.Select(u => new
                {
                    UserId = u.Id,
                    Username = u.Username,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname
                });

                return Ok(userList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving online users");
                return StatusCode(500, new { Error = "An unexpected error occurred" });
            }
        }
    }
}