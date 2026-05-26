using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ThreatPilot.Frontend.Models;

namespace ThreatPilot.Frontend.ViewModels
{
    public partial class IncidentViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _alertId = string.Empty;

        [ObservableProperty]
        private string _ruleName = string.Empty;

        [ObservableProperty]
        private string _severity = string.Empty;

        [ObservableProperty]
        private string _timestamp = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _sourceIp = string.Empty;

        [ObservableProperty]
        private string _userId = string.Empty;

        [ObservableProperty]
        private string _status = string.Empty;

        [ObservableProperty]
        private string _aiSummary = string.Empty;

        [ObservableProperty]
        private string _aiSeverityReason = string.Empty;

        public ObservableCollection<string> RecommendedActions { get; } = new();

        public IncidentViewModel()
        {
            // Default dummy data if no alert was selected yet
            AlertId = "ALR-000";
            RuleName = "No Incident Selected";
            Severity = "LOW";
            Timestamp = System.DateTime.Now.ToString("dddd, MMMM dd, yyyy  HH:mm:ss");
            Description = "Please select an alert from the Threat Center to view its details here.";
            SourceIp = "N/A";
            UserId = "N/A";
            Status = "Resolved";
            AiSummary = "No AI analysis available because no incident is currently selected.";
            AiSeverityReason = "N/A";
            RecommendedActions.Add("Navigate to Threat Center");
            RecommendedActions.Add("Select an active alert");
        }

        public void LoadAlert(AlertModel alert)
        {
            AlertId = alert.AlertId;
            RuleName = alert.RuleName;
            Severity = alert.Severity;
            Timestamp = alert.Timestamp.ToString("dddd, MMMM dd, yyyy  HH:mm:ss");
            Description = alert.Description;
            SourceIp = alert.SourceIp;
            UserId = alert.UserId;
            Status = alert.Status;
            AiSummary = alert.AiSummary;
            AiSeverityReason = alert.AiSeverityReason;

            RecommendedActions.Clear();
            foreach (var action in alert.AiRecommendedActions)
                RecommendedActions.Add(action);
        }
    }
}
