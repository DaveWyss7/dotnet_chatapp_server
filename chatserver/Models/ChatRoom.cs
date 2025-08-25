using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models
{
    public class ChatRoom
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}