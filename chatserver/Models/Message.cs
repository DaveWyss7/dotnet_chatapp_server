using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ChatRoomId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public User User { get; set; } = null!;
        public ChatRoom ChatRoom { get; set; } = null!;
    }
}