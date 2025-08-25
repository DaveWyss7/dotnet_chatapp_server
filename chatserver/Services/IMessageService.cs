using ChatServer.Models;
using ChatServer.Models.DTOs;

namespace ChatServer.Services
{
    public interface IMessageService
    {
        Task<Message> CreateMessageAsync(int userId, SendMessageDto messageDto);
        Task<IEnumerable<MessageResponseDto>> GetMessagesForRoomAsync(int chatRoomId, int limit = 50);
    }
}