using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.DTOs
{
    public class SendMessageDto
    {
        [Required, StringLength(1000, MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int ChatRoomId { get; set; }
    }
}