using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreatPilot.Backend.Models
{
    public class SecurityEvent
    {
        [Key]
        public Guid EventId { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string SourceIp { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string EventType { get; set; } = string.Empty; // e.g., "login_failed"

        [MaxLength(100)]
        public string? TargetResource { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty; // e.g., "failed", "success"

        [Column(TypeName = "jsonb")]
        public string? Metadata { get; set; } // Stored as JSON string
    }
}
