using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreatPilot.Backend.Models
{
    public class Alert
    {
        [Key]
        public Guid AlertId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string RuleName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Severity { get; set; } = string.Empty; // LOW, MEDIUM, HIGH, CRITICAL

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? SourceIp { get; set; }

        [MaxLength(100)]
        public string? UserId { get; set; }

        // Associated logs stored as JSON array of strings
        [Column(TypeName = "jsonb")]
        public string? TriggeringLogs { get; set; } 

        // AI Enrichment fields
        public string? AiSummary { get; set; }
        public string? AiSeverityReason { get; set; }
        
        [Column(TypeName = "jsonb")]
        public string? AiRecommendedActions { get; set; } // Stored as JSON array of strings
        
        [MaxLength(20)]
        public string Status { get; set; } = "Open"; // Open, Investigating, Resolved, FalsePositive
    }
}
