using Microsoft.EntityFrameworkCore;
using ChatServer.Data;
using ChatServer.Models;
using ChatServer.Models.DTOs;

namespace ChatServer.Services
{
    public class MessageService : IMessageService
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<MessageService> _logger;

        public MessageService(ChatDbContext context, ILogger<MessageService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Message> CreateMessageAsync(int userId, SendMessageDto messageDto)
        {
            try
            {
                var message = new Message
                {
                    Content = messageDto.Content,
                    UserId = userId,
                    ChatRoomId = messageDto.ChatRoomId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                // Load related data
                await _context.Entry(message)
                    .Reference(m => m.User)
                    .LoadAsync();

                await _context.Entry(message)
                    .Reference(m => m.ChatRoom)
                    .LoadAsync();

                _logger.LogInformation("Message created by user {UserId} in room {ChatRoomId}", 
                    userId, messageDto.ChatRoomId);

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating message for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<MessageResponseDto>> GetMessagesForRoomAsync(int chatRoomId, int limit = 50)
        {
            try
            {
                var messages = await _context.Messages
                    .Where(m => m.ChatRoomId == chatRoomId)
                    .Include(m => m.User)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(limit)
                    .Select(m => new MessageResponseDto
                    {
                        Id = m.Id,
                        Content = m.Content,
                        Username = m.User.Username,
                        ChatRoomId = m.ChatRoomId,
                        CreatedAt = m.CreatedAt
                    })
                    .ToListAsync();

                return messages.AsEnumerable().Reverse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages for room {ChatRoomId}", chatRoomId);
                throw;
            }
        }
    }
}