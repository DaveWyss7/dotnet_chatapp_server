// TODO: Erstelle Data/ChatDbContext.cs
using Microsoft.EntityFrameworkCore;
using ChatServer.Models;

namespace ChatServer.Data
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        // DbSets - repräsentieren Datenbank-Tabellen
        public DbSet<User> Users { get; set; }
        // public DbSet<Message> Messages { get; set; }  // Später in Phase 4

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Entity Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Firstname).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Lastname).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password_Hash).IsRequired().HasMaxLength(255);

                // Unique Constraints
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                // Table Name
                entity.ToTable("users");
            });
        }
    }
}