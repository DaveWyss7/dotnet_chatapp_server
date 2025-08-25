using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using ChatServer.Services;
using ChatServer.Models.DTOs;

namespace ChatServer.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IPresenceService _presenceService;
        private readonly IUserService _userService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(
            IMessageService messageService,
            IPresenceService presenceService,
            IUserService userService,
            ILogger<ChatHub> logger)
        {
            _messageService = messageService;
            _presenceService = presenceService;
            _userService = userService;
            _logger = logger;
        }

        // ✅ User Connection
        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                try
                {
                    await _presenceService.ConnectUserAsync(userId.Value, Context.ConnectionId);
                    
                    var user = await _userService.GetUserByIdAsync(userId.Value);
                    if (user != null)
                    {
                        await Clients.All.SendAsync("UserOnline", new
                        {
                            UserId = user.Id,
                            Username = user.Username
                        });

                        _logger.LogInformation("User {Username} connected to chat", user.Username);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling connection for user {UserId}", userId);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                await _presenceService.DisconnectUserAsync(Context.ConnectionId);
                
                var userId = GetUserId();
                if (userId.HasValue)
                {
                    var user = await _userService.GetUserByIdAsync(userId.Value);
                    if (user != null)
                    {
                        await Clients.All.SendAsync("UserOffline", new
                        {
                            UserId = user.Id,
                            Username = user.Username
                        });

                        _logger.LogInformation("User {Username} disconnected", user.Username);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling disconnection");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // ✅ Chat Room Management
        public async Task JoinRoom(int chatRoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Room_{chatRoomId}");
            
            var userId = GetUserId();
            if (userId.HasValue)
            {
                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user != null)
                {
                    await Clients.Group($"Room_{chatRoomId}").SendAsync("UserJoinedRoom", new
                    {
                        ChatRoomId = chatRoomId,
                        UserId = user.Id,
                        Username = user.Username
                    });
                }
            }
        }

        public async Task LeaveRoom(int chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Room_{chatRoomId}");
            
            var userId = GetUserId();
            if (userId.HasValue)
            {
                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user != null)
                {
                    await Clients.Group($"Room_{chatRoomId}").SendAsync("UserLeftRoom", new
                    {
                        ChatRoomId = chatRoomId,
                        UserId = user.Id,
                        Username = user.Username
                    });
                }
            }
        }

        // ✅ Send Message
        public async Task SendMessage(SendMessageDto messageDto)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            try
            {
                var message = await _messageService.CreateMessageAsync(userId.Value, messageDto);

                var messageResponse = new MessageResponseDto
                {
                    Id = message.Id,
                    Content = message.Content,
                    Username = message.User.Username,
                    ChatRoomId = message.ChatRoomId,
                    CreatedAt = message.CreatedAt
                };

                await Clients.Group($"Room_{message.ChatRoomId}")
                    .SendAsync("ReceiveMessage", messageResponse);

                _logger.LogInformation("Message sent by {Username}", message.User.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                await Clients.Caller.SendAsync("Error", "Failed to send message");
            }
        }

        // ✅ Typing Indicator
        public async Task SendTypingIndicator(int chatRoomId, bool isTyping)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user != null)
            {
                await Clients.GroupExcept($"Room_{chatRoomId}", Context.ConnectionId)
                    .SendAsync("TypingIndicator", new
                    {
                        ChatRoomId = chatRoomId,
                        Username = user.Username,
                        IsTyping = isTyping
                    });
            }
        }

        // ✅ Helper Method
        private int? GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}