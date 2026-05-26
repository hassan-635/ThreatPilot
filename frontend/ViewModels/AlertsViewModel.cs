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
            ThreatPilot.Frontend.Services.BackendService.Instance.OnAlertReceived += OnAlertReceived;
            LoadInitialAlerts();
        }

        private async void LoadInitialAlerts()
        {
            var alerts = await ThreatPilot.Frontend.Services.BackendService.Instance.GetAlertsAsync();
            foreach (var alert in alerts)
            {
                AllAlerts.Add(alert);
            }
            ApplyFilter();
        }

        private void OnAlertReceived(AlertModel alert)
        {
            AllAlerts.Insert(0, alert);
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

        // Dummy alerts removed
    }
}
