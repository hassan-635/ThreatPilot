using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System;
using ThreatPilot.Frontend.Models;

namespace ThreatPilot.Frontend.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _currentView = "Dashboard";

        [ObservableProperty]
        private string _currentUser = "Hassan Ahmed";

        [ObservableProperty]
        private string _currentRole = "SOC Analyst";

        [ObservableProperty]
        private object? _currentPage;

        public DashboardViewModel DashboardVM { get; }
        public AlertsViewModel AlertsVM { get; }
        public IncidentViewModel IncidentVM { get; }
        public SettingsViewModel SettingsVM { get; }

        public MainViewModel()
        {
            DashboardVM = new DashboardViewModel();
            AlertsVM = new AlertsViewModel();
            IncidentVM = new IncidentViewModel();
            SettingsVM = new SettingsViewModel();
            CurrentPage = DashboardVM;
        }

        [RelayCommand]
        private void NavigateTo(string page)
        {
            CurrentView = page;
            CurrentPage = page switch
            {
                "Dashboard" => DashboardVM,
                "Alerts" => AlertsVM,
                "Incident" => IncidentVM,
                "Settings" => SettingsVM,
                _ => DashboardVM
            };
        }

        [RelayCommand]
        private void ViewIncident(AlertModel? alert)
        {
            if (alert != null)
            {
                IncidentVM.LoadAlert(alert);
                NavigateTo("Incident");
            }
        }
    }
}
