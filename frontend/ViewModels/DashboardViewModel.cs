using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using ThreatPilot.Frontend.Models;

namespace ThreatPilot.Frontend.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        public ObservableCollection<MetricModel> Metrics { get; } = new();
        public ObservableCollection<ActivityModel> RecentActivity { get; } = new();
        public ObservableCollection<AlertModel> CriticalAlerts { get; } = new();

        // NEW: Threat Distribution
        public ObservableCollection<ThreatDistItem> ThreatDistribution { get; } = new();

        // NEW: System Health
        public ObservableCollection<SystemHealthItem> SystemHealth { get; } = new();

        [ObservableProperty]
        private string _systemStatus = "Secure";

        [ObservableProperty]
        private string _lastScanTime = "2 minutes ago";

        [ObservableProperty]
        private string _currentDateTime = DateTime.Now.ToString("dddd, MMMM dd yyyy  •  HH:mm");

        [ObservableProperty]
        private int _totalAlertsToday = 12;

        [ObservableProperty]
        private int _resolvedToday = 9;

        [ObservableProperty]
        private double _resolutionRate = 75.0;

        [RelayCommand]
        private void GenerateReport()
        {
            try
            {
                string folder = SettingsViewModel.GlobalReportSaveLocation;
                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }

                string fileName = $"ThreatPilot_Report_{System.DateTime.Now:yyyyMMdd_HHmmss}.html";
                string fullPath = System.IO.Path.Combine(folder, fileName);

                string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <title>ThreatPilot Executive Summary</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; padding: 40px; background-color: #F8F9FA; color: #16191F; }}
        .header {{ border-bottom: 2px solid #D4AF37; padding-bottom: 20px; margin-bottom: 30px; }}
        h1 {{ color: #16191F; }}
        .metric-box {{ background: white; padding: 20px; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); margin-bottom: 20px; }}
        .critical {{ color: #DC2626; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>🛡 ThreatPilot Executive Summary</h1>
        <p>Generated on: {System.DateTime.Now:F}</p>
    </div>
    <div class='metric-box'>
        <h2>System Status: Secure</h2>
        <p>Resolution Rate: {ResolutionRate}%</p>
        <p>Active Threats: <span class='critical'>3</span></p>
        <p>Total Alerts Today: {TotalAlertsToday}</p>
    </div>
    <div class='metric-box'>
        <h2>Recent Critical Alerts</h2>
        <ul>
            <li>[ALR-001] Brute Force Detection (10.0.0.5)</li>
            <li>[ALR-002] Port Scanning Activity (192.168.1.45)</li>
        </ul>
    </div>
</body>
</html>";

                System.IO.File.WriteAllText(fullPath, htmlContent);

                System.Windows.MessageBox.Show(
                    $"Executive Summary Report has been generated successfully.\n\nFile saved to: {fullPath}",
                    "Report Generated",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);

                // Open the report automatically
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fullPath,
                    UseShellExecute = true
                });
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to generate report: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public DashboardViewModel()
        {
            LoadDummyData();
        }

        private void LoadDummyData()
        {
            // Metrics
            Metrics.Add(new MetricModel { Title = "Active Threats", Value = "3", Icon = "⚠", Trend = "+1 from yesterday", IsPositive = false });
            Metrics.Add(new MetricModel { Title = "Events Processed", Value = "12,847", Icon = "📊", Trend = "+18% this week", IsPositive = true });
            Metrics.Add(new MetricModel { Title = "Monitored Assets", Value = "156", Icon = "🖥", Trend = "All systems online", IsPositive = true });
            Metrics.Add(new MetricModel { Title = "AI Analyses", Value = "47", Icon = "🧠", Trend = "+5 today", IsPositive = true });

            // Recent Activity
            RecentActivity.Add(new ActivityModel { Timestamp = DateTime.Now.AddMinutes(-2), EventType = "Brute Force", Description = "10+ failed login attempts from 10.0.0.5", SourceIp = "10.0.0.5", Severity = "CRITICAL" });
            RecentActivity.Add(new ActivityModel { Timestamp = DateTime.Now.AddMinutes(-8), EventType = "Port Scan", Description = "Sequential port scanning detected on subnet 192.168.1.x", SourceIp = "192.168.1.45", Severity = "HIGH" });
            RecentActivity.Add(new ActivityModel { Timestamp = DateTime.Now.AddMinutes(-15), EventType = "Login Success", Description = "Admin login from authorized workstation", SourceIp = "10.0.1.10", Severity = "LOW" });
            RecentActivity.Add(new ActivityModel { Timestamp = DateTime.Now.AddMinutes(-22), EventType = "Privilege Escalation", Description = "User 'jdoe' attempted privilege escalation via sudo", SourceIp = "10.0.0.12", Severity = "HIGH" });
            RecentActivity.Add(new ActivityModel { Timestamp = DateTime.Now.AddMinutes(-30), EventType = "Resource Access", Description = "Unusual access pattern to /etc/shadow file", SourceIp = "10.0.0.12", Severity = "MEDIUM" });
            RecentActivity.Add(new ActivityModel { Timestamp = DateTime.Now.AddHours(-1), EventType = "Login Failed", Description = "Multiple credential stuffing attempts", SourceIp = "203.0.113.50", Severity = "MEDIUM" });

            // Critical Alerts
            CriticalAlerts.Add(new AlertModel
            {
                AlertId = "ALR-001", RuleName = "Brute Force Detection", Severity = "CRITICAL",
                Timestamp = DateTime.Now.AddMinutes(-2),
                Description = "Multiple failed login attempts detected from single IP address targeting SSH service.",
                SourceIp = "10.0.0.5", Status = "Open",
                AiSummary = "A sustained brute force attack originating from internal IP 10.0.0.5 has been detected."
            });
            CriticalAlerts.Add(new AlertModel
            {
                AlertId = "ALR-002", RuleName = "Port Scanning Activity", Severity = "HIGH",
                Timestamp = DateTime.Now.AddMinutes(-8),
                Description = "Sequential port scanning detected across subnet 192.168.1.x from a single source.",
                SourceIp = "192.168.1.45", Status = "Investigating",
                AiSummary = "Reconnaissance activity detected. An internal host is systematically probing open ports."
            });

            // NEW: Threat Distribution
            ThreatDistribution.Add(new ThreatDistItem { Category = "Brute Force", Count = 34, Percentage = 40 });
            ThreatDistribution.Add(new ThreatDistItem { Category = "Port Scanning", Count = 22, Percentage = 26 });
            ThreatDistribution.Add(new ThreatDistItem { Category = "Credential Stuffing", Count = 15, Percentage = 18 });
            ThreatDistribution.Add(new ThreatDistItem { Category = "Privilege Escalation", Count = 8, Percentage = 9 });
            ThreatDistribution.Add(new ThreatDistItem { Category = "Other", Count = 6, Percentage = 7 });

            // NEW: System Health
            SystemHealth.Add(new SystemHealthItem { Name = "Firewall", Status = "Online", Uptime = "99.97%", StatusColor = "GREEN" });
            SystemHealth.Add(new SystemHealthItem { Name = "IDS/IPS", Status = "Online", Uptime = "99.92%", StatusColor = "GREEN" });
            SystemHealth.Add(new SystemHealthItem { Name = "SIEM Collector", Status = "Warning", Uptime = "98.45%", StatusColor = "YELLOW" });
            SystemHealth.Add(new SystemHealthItem { Name = "AI Engine", Status = "Online", Uptime = "99.99%", StatusColor = "GREEN" });
            SystemHealth.Add(new SystemHealthItem { Name = "Database", Status = "Online", Uptime = "99.95%", StatusColor = "GREEN" });
        }
    }

    // NEW supporting models
    public class ThreatDistItem
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class SystemHealthItem
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Uptime { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
    }
}
