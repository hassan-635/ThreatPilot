using System;
using System.Collections.Generic;

namespace ThreatPilot.Frontend.Models
{
    public class AlertModel
    {
        public string AlertId { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SourceIp { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AiSummary { get; set; } = string.Empty;
        public string AiSeverityReason { get; set; } = string.Empty;
        public List<string> AiRecommendedActions { get; set; } = new();
    }

    public class MetricModel
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Trend { get; set; } = string.Empty;
        public bool IsPositive { get; set; }
    }

    public class ActivityModel
    {
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SourceIp { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
    }
}
