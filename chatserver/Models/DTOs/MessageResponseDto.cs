namespace ChatServer.Models.DTOs
{
    public class MessageResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int ChatRoomId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}