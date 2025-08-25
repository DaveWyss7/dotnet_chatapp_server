using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.DTOs
{
    public class CreateChatRoomDto
    {
        [Required, StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}