using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models
{
    public class UserSession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, StringLength(200)]
        public string ConnectionId { get; set; } = string.Empty;

        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

        public bool IsOnline { get; set; } = true;

        // Navigation Properties
        public User User { get; set; } = null!;
    }
}