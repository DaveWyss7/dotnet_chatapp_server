using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models
{
    public class User
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(50)]
    public required string Username { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public required string Firstname { get; set; }

    [Required, StringLength(50)]
    public required string Lastname { get; set; }

    [Required, EmailAddress, StringLength(100)]
    public required string Email { get; set; }
    
    [Required, StringLength(100)]
    public required string Password_Hash { get; set; }
}
}
    


