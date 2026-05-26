using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        /// <summary>
        /// The backend stores this as a JSON string (e.g. "[\"action1\",\"action2\"]").
        /// This converter handles both a raw JSON array and an already-serialized JSON string.
        /// </summary>
        [JsonConverter(typeof(JsonStringListConverter))]
        public List<string> AiRecommendedActions { get; set; } = new();
    }

    /// <summary>
    /// Handles deserializing AiRecommendedActions which comes from the backend
    /// as a JSON-encoded string (e.g. "[\"block IP\",\"reset creds\"]") rather 
    /// than a native JSON array.
    /// </summary>
    public class JsonStringListConverter : JsonConverter<List<string>>
    {
        public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return new List<string>();

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Native JSON array — just deserialize normally
                var list = new List<string>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    list.Add(reader.GetString() ?? string.Empty);
                }
                return list;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                // It's a JSON-encoded string — parse the inner JSON
                var jsonString = reader.GetString();
                if (string.IsNullOrEmpty(jsonString))
                    return new List<string>();

                try
                {
                    return JsonSerializer.Deserialize<List<string>>(jsonString) ?? new List<string>();
                }
                catch
                {
                    return new List<string> { jsonString };
                }
            }

            return new List<string>();
        }

        public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
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
