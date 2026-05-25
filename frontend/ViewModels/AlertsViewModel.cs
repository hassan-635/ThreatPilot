using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using ThreatPilot.Frontend.Models;

namespace ThreatPilot.Frontend.ViewModels
{
    public partial class AlertsViewModel : ObservableObject
    {
        public ObservableCollection<AlertModel> AllAlerts { get; } = new();
        public ObservableCollection<AlertModel> FilteredAlerts { get; } = new();

        [ObservableProperty]
        private string _selectedFilter = "All";

        [ObservableProperty]
        private string _searchText = string.Empty;

        public AlertsViewModel()
        {
            LoadDummyAlerts();
            ApplyFilter();
        }

        partial void OnSelectedFilterChanged(string value) => ApplyFilter();
        partial void OnSearchTextChanged(string value) => ApplyFilter();

        [RelayCommand]
        private void SetFilter(string filter)
        {
            SelectedFilter = filter;
        }

        private void ApplyFilter()
        {
            FilteredAlerts.Clear();
            var results = AllAlerts.AsEnumerable();

            if (SelectedFilter != "All")
                results = results.Where(a => a.Severity == SelectedFilter);

            if (!string.IsNullOrWhiteSpace(SearchText))
                results = results.Where(a =>
                    a.RuleName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    a.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    a.SourceIp.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var alert in results)
                FilteredAlerts.Add(alert);
        }

        private void LoadDummyAlerts()
        {
            AllAlerts.Add(new AlertModel
            {
                AlertId = "ALR-001",
                RuleName = "Brute Force Detection",
                Severity = "CRITICAL",
                Timestamp = DateTime.Now.AddMinutes(-2),
                Description = "Multiple failed login attempts detected from single IP address targeting SSH service.",
                SourceIp = "10.0.0.5",
                UserId = "root",
                Status = "Open",
                AiSummary = "A sustained brute force attack originating from internal IP 10.0.0.5 has been detected targeting the SSH authentication service. The attacker made 15 failed attempts within a 2-minute window, suggesting automated password-guessing tool usage.",
                AiSeverityReason = "This is classified as CRITICAL because the attack originates from an internal IP, suggesting a compromised host or insider threat. The target is the SSH service which provides shell access.",
                AiRecommendedActions = new() { "Immediately block IP 10.0.0.5 at the firewall level", "Reset credentials for the 'root' account", "Investigate the source machine for compromise indicators", "Enable account lockout policies for SSH", "Deploy multi-factor authentication for privileged accounts" }
            });

            AllAlerts.Add(new AlertModel
            {
                AlertId = "ALR-002",
                RuleName = "Port Scanning Activity",
                Severity = "HIGH",
                Timestamp = DateTime.Now.AddMinutes(-8),
                Description = "Sequential port scanning detected across subnet 192.168.1.x from a single source.",
                SourceIp = "192.168.1.45",
                UserId = "unknown",
                Status = "Investigating",
                AiSummary = "Reconnaissance activity detected from internal host 192.168.1.45. The source is systematically probing ports 1-1024 across multiple hosts in the 192.168.1.0/24 subnet.",
                AiSeverityReason = "Classified as HIGH because port scanning is typically a precursor to lateral movement or exploitation. The internal origin increases risk significantly.",
                AiRecommendedActions = new() { "Isolate host 192.168.1.45 from the network", "Perform forensic analysis on the scanning host", "Review firewall rules for internal segmentation", "Check for recently installed unauthorized software" }
            });

            AllAlerts.Add(new AlertModel
            {
                AlertId = "ALR-003",
                RuleName = "Credential Stuffing",
                Severity = "HIGH",
                Timestamp = DateTime.Now.AddMinutes(-15),
                Description = "Multiple login attempts using different usernames from the same external IP.",
                SourceIp = "203.0.113.50",
                UserId = "multiple",
                Status = "Open",
                AiSummary = "External IP 203.0.113.50 attempted to authenticate with 23 different usernames within 5 minutes, consistent with credential stuffing attack patterns.",
                AiSeverityReason = "Classified as HIGH due to the external origin and the volume of unique credentials being tested, indicating a database breach somewhere in the supply chain.",
                AiRecommendedActions = new() { "Block IP 203.0.113.50 immediately", "Force password resets for any accounts that received login attempts", "Implement CAPTCHA on the login form", "Check if any of the tested credentials were valid" }
            });

            AllAlerts.Add(new AlertModel
            {
                AlertId = "ALR-004",
                RuleName = "Privilege Escalation Attempt",
                Severity = "MEDIUM",
                Timestamp = DateTime.Now.AddMinutes(-22),
                Description = "User 'jdoe' attempted unauthorized privilege escalation via sudo command.",
                SourceIp = "10.0.0.12",
                UserId = "jdoe",
                Status = "Resolved",
                AiSummary = "User 'jdoe' attempted to execute 'sudo su -' and 'sudo cat /etc/shadow' commands without proper authorization. Both attempts were denied by the PAM module.",
                AiSeverityReason = "Classified as MEDIUM because the attempts were denied and no actual escalation occurred, but the intent suggests policy violation or compromised account.",
                AiRecommendedActions = new() { "Interview user 'jdoe' about the unauthorized sudo attempts", "Review jdoe's recent activity logs for other suspicious behavior", "Confirm jdoe's account has not been compromised" }
            });

            AllAlerts.Add(new AlertModel
            {
                AlertId = "ALR-005",
                RuleName = "Unusual Resource Access",
                Severity = "LOW",
                Timestamp = DateTime.Now.AddHours(-1),
                Description = "Unusual file access pattern detected on sensitive configuration files.",
                SourceIp = "10.0.1.10",
                UserId = "admin",
                Status = "Resolved",
                AiSummary = "Admin user accessed multiple configuration files in /etc/ directory in rapid succession. Pattern is consistent with routine maintenance but occurred outside normal working hours.",
                AiSeverityReason = "Classified as LOW because the user has legitimate admin permissions, but the timing is unusual and warrants documentation.",
                AiRecommendedActions = new() { "Verify admin was performing scheduled maintenance", "Log the activity for audit purposes" }
            });
        }
    }
}
