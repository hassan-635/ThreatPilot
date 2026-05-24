using System.Text.Json.Serialization;

namespace ThreatPilot.Backend.DTOs
{
    public class LogEventDto
    {
        [JsonPropertyName("event_id")]
        public string EventId { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("source_ip")]
        public string SourceIp { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("event_type")]
        public string EventType { get; set; } = string.Empty;

        [JsonPropertyName("target_resource")]
        public string? TargetResource { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class LogBatchDto
    {
        [JsonPropertyName("logs")]
        public List<LogEventDto> Logs { get; set; } = new();
    }

    public class AlertDto
    {
        [JsonPropertyName("alert_id")]
        public string AlertId { get; set; } = string.Empty;

        [JsonPropertyName("rule_name")]
        public string RuleName { get; set; } = string.Empty;

        [JsonPropertyName("severity")]
        public string Severity { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("source_ip")]
        public string? SourceIp { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("triggering_logs")]
        public List<LogEventDto> TriggeringLogs { get; set; } = new();
    }

    public class AIReportDto
    {
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("severity_reason")]
        public string SeverityReason { get; set; } = string.Empty;

        [JsonPropertyName("recommended_actions")]
        public List<string> RecommendedActions { get; set; } = new();
    }

    public class IncidentAnalysisRequestDto
    {
        [JsonPropertyName("alert")]
        public AlertDto Alert { get; set; } = new();
    }
}
