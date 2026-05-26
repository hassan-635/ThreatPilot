using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        private async Task ClearAlerts()
        {
            var result = System.Windows.MessageBox.Show(
                "Are you sure you want to clear all alerts from the database? This is for testing purposes only and cannot be undone.",
                "Clear All Alerts",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                bool success = await ThreatPilot.Frontend.Services.BackendService.Instance.ClearAlertsAsync();
                if (success)
                {
                    // Clear UI collections and reset metrics
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        CriticalAlerts.Clear();
                        RecentActivity.Clear();
                        Metrics.Clear();
                        ThreatDistribution.Clear();
                        TotalAlertsToday = 0;
                        ResolvedToday = 0;
                        ResolutionRate = 0;
                        SystemStatus = "Secure";
                        LastScanTime = "Just now";
                        
                        // Re-add empty/default metrics
                        Metrics.Add(new MetricModel { Title = "Active Threats", Value = "0", Icon = "⚠", Trend = "0 total alerts", IsPositive = true });
                        Metrics.Add(new MetricModel { Title = "Events Processed", Value = "0", Icon = "📊", Trend = "From backend DB", IsPositive = true });
                        Metrics.Add(new MetricModel { Title = "Resolved", Value = "0", Icon = "✅", Trend = "No alerts", IsPositive = true });
                        Metrics.Add(new MetricModel { Title = "AI Analyses", Value = "0", Icon = "🧠", Trend = "AI-enriched alerts", IsPositive = true });
                    });
                }
                else
                {
                    System.Windows.MessageBox.Show("Failed to clear alerts from backend.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

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

                // Build critical alerts list dynamically
                var alertRows = new System.Text.StringBuilder();
                foreach (var a in CriticalAlerts)
                {
                    alertRows.AppendLine($"            <tr><td>{a.AlertId}</td><td>{a.RuleName}</td><td><span class='{a.Severity.ToLower()}'>{a.Severity}</span></td><td>{a.SourceIp}</td><td>{a.Description}</td></tr>");
                }

                // Build threat distribution rows
                var distRows = new System.Text.StringBuilder();
                foreach (var t in ThreatDistribution)
                {
                    distRows.AppendLine($"            <tr><td>{t.Category}</td><td>{t.Count}</td><td>{t.Percentage}%</td></tr>");
                }

                string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <title>ThreatPilot Executive Summary</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; padding: 40px; background-color: #F8F9FA; color: #16191F; }}
        .header {{ border-bottom: 3px solid #D4AF37; padding-bottom: 20px; margin-bottom: 30px; }}
        h1 {{ color: #16191F; margin: 0; }}
        h2 {{ color: #1E293B; border-bottom: 1px solid #E2E8F0; padding-bottom: 8px; }}
        .metric-grid {{ display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; margin-bottom: 30px; }}
        .metric-box {{ background: white; padding: 20px; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); text-align: center; }}
        .metric-box .value {{ font-size: 2em; font-weight: bold; color: #D4AF37; }}
        .metric-box .label {{ color: #64748B; margin-top: 4px; }}
        .section {{ background: white; padding: 20px; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); margin-bottom: 20px; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 10px; }}
        th, td {{ text-align: left; padding: 10px 12px; border-bottom: 1px solid #E2E8F0; font-size: 0.9em; }}
        th {{ background: #F1F5F9; font-weight: 600; }}
        .critical {{ color: #DC2626; font-weight: bold; }}
        .high {{ color: #EA580C; font-weight: bold; }}
        .medium {{ color: #CA8A04; font-weight: bold; }}
        .low {{ color: #16A34A; font-weight: bold; }}
        .footer {{ margin-top: 30px; text-align: center; color: #94A3B8; font-size: 0.85em; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>🛡 ThreatPilot Executive Summary</h1>
        <p style='color:#64748B;'>Generated on: {System.DateTime.Now:F}</p>
    </div>
    <div class='metric-grid'>
        <div class='metric-box'><div class='value'>{SystemStatus}</div><div class='label'>System Status</div></div>
        <div class='metric-box'><div class='value'>{TotalAlertsToday}</div><div class='label'>Total Alerts</div></div>
        <div class='metric-box'><div class='value'>{ResolvedToday}</div><div class='label'>Resolved</div></div>
        <div class='metric-box'><div class='value'>{ResolutionRate}%</div><div class='label'>Resolution Rate</div></div>
    </div>
    <div class='section'>
        <h2>Critical &amp; High Alerts</h2>
        <table>
            <tr><th>Alert ID</th><th>Rule</th><th>Severity</th><th>Source IP</th><th>Description</th></tr>
{alertRows}
        </table>
    </div>
    <div class='section'>
        <h2>Threat Distribution</h2>
        <table>
            <tr><th>Category</th><th>Count</th><th>Share</th></tr>
{distRows}
        </table>
    </div>
    <div class='footer'>ThreatPilot — AI-Powered SOC Platform</div>
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
            ThreatPilot.Frontend.Services.BackendService.Instance.OnAlertReceived += OnAlertReceived;
            LoadInitialData();
        }

        private async void LoadInitialData()
        {
            var alerts = await ThreatPilot.Frontend.Services.BackendService.Instance.GetAlertsAsync();

            // Populate Critical Alerts
            foreach (var alert in alerts)
            {
                if (alert.Severity == "CRITICAL" || alert.Severity == "HIGH")
                {
                    CriticalAlerts.Add(alert);
                }
            }

            // Populate Recent Activity from all alerts
            foreach (var alert in alerts)
            {
                RecentActivity.Add(new ActivityModel
                {
                    Timestamp = alert.Timestamp,
                    EventType = alert.RuleName,
                    Description = alert.Description,
                    SourceIp = alert.SourceIp ?? "N/A",
                    Severity = alert.Severity
                });
            }

            // Compute Metrics from real data
            int totalAlerts = alerts.Count;
            int criticalCount = alerts.Count(a => a.Severity == "CRITICAL" || a.Severity == "HIGH");
            int resolvedCount = alerts.Count(a => a.Status == "Resolved");
            int aiAnalyses = alerts.Count(a => !string.IsNullOrEmpty(a.AiSummary));

            Metrics.Add(new MetricModel { Title = "Active Threats", Value = criticalCount.ToString(), Icon = "⚠", Trend = $"{totalAlerts} total alerts", IsPositive = criticalCount == 0 });
            Metrics.Add(new MetricModel { Title = "Events Processed", Value = totalAlerts.ToString(), Icon = "📊", Trend = "From backend DB", IsPositive = true });
            Metrics.Add(new MetricModel { Title = "Resolved", Value = resolvedCount.ToString(), Icon = "✅", Trend = totalAlerts > 0 ? $"{(resolvedCount * 100 / totalAlerts)}% resolution" : "No alerts", IsPositive = true });
            Metrics.Add(new MetricModel { Title = "AI Analyses", Value = aiAnalyses.ToString(), Icon = "🧠", Trend = "AI-enriched alerts", IsPositive = true });

            // Update dashboard counters
            TotalAlertsToday = totalAlerts;
            ResolvedToday = resolvedCount;
            ResolutionRate = totalAlerts > 0 ? Math.Round((double)resolvedCount / totalAlerts * 100, 1) : 0;
            SystemStatus = criticalCount > 0 ? "At Risk" : "Secure";
            LastScanTime = "Just now";

            // Compute Threat Distribution from real data
            var grouped = alerts.GroupBy(a => a.RuleName).OrderByDescending(g => g.Count());
            foreach (var group in grouped)
            {
                ThreatDistribution.Add(new ThreatDistItem
                {
                    Category = group.Key,
                    Count = group.Count(),
                    Percentage = totalAlerts > 0 ? Math.Round((double)group.Count() / totalAlerts * 100, 0) : 0
                });
            }

            // System Health (static for now — no backend endpoint for this yet)
            SystemHealth.Add(new SystemHealthItem { Name = "Firewall", Status = "Online", Uptime = "99.97%", StatusColor = "GREEN" });
            SystemHealth.Add(new SystemHealthItem { Name = "IDS/IPS", Status = "Online", Uptime = "99.92%", StatusColor = "GREEN" });
            SystemHealth.Add(new SystemHealthItem { Name = "SIEM Collector", Status = "Online", Uptime = "98.45%", StatusColor = "GREEN" });
            SystemHealth.Add(new SystemHealthItem { Name = "AI Engine", Status = "Online", Uptime = "99.99%", StatusColor = "GREEN" });
            SystemHealth.Add(new SystemHealthItem { Name = "Database", Status = "Online", Uptime = "99.95%", StatusColor = "GREEN" });
        }

        private void OnAlertReceived(AlertModel alert)
        {
            // Add to critical alerts if severity is high enough
            if (alert.Severity == "CRITICAL" || alert.Severity == "HIGH")
            {
                CriticalAlerts.Insert(0, alert);
                if (CriticalAlerts.Count > 10)
                    CriticalAlerts.RemoveAt(CriticalAlerts.Count - 1);
            }

            // Add to recent activity
            RecentActivity.Insert(0, new ActivityModel
            {
                Timestamp = alert.Timestamp,
                EventType = alert.RuleName,
                Description = alert.Description,
                SourceIp = alert.SourceIp ?? "N/A",
                Severity = alert.Severity
            });

            // Update counters
            TotalAlertsToday++;
            if (alert.Status == "Resolved") ResolvedToday++;
            ResolutionRate = TotalAlertsToday > 0 ? Math.Round((double)ResolvedToday / TotalAlertsToday * 100, 1) : 0;
            SystemStatus = (alert.Severity == "CRITICAL" || alert.Severity == "HIGH") ? "At Risk" : SystemStatus;
        }

        // Dummy data loading removed.
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
