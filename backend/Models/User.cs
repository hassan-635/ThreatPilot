using System.ComponentModel.DataAnnotations;

namespace ThreatPilot.Backend.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Analyst"; // e.g. "Admin", "Analyst"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
