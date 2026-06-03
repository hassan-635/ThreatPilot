using CommunityToolkit.Mvvm.ComponentModel;

namespace ThreatPilot.Frontend.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isDarkMode = false;

        [ObservableProperty]
        private bool _enableDesktopNotifications = true;

        [ObservableProperty]
        private bool _autoRefreshAlerts = true;

        [ObservableProperty]
        private string _aiEngineEndpoint = "http://localhost:8000/api/v1/";

        public static string GlobalReportSaveLocation = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "ThreatPilot");

        [ObservableProperty]
        private string _reportSaveLocation = GlobalReportSaveLocation;

        partial void OnReportSaveLocationChanged(string value)
        {
            GlobalReportSaveLocation = value;
        }

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        private void BrowseFolder()
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Select Report Save Location",
                InitialDirectory = ReportSaveLocation
            };

            if (dialog.ShowDialog() == true)
            {
                ReportSaveLocation = dialog.FolderName;
            }
        }

        partial void OnIsDarkModeChanged(bool value)
        {
            var app = System.Windows.Application.Current;
            var dictionaries = app.Resources.MergedDictionaries;
            
            // Assuming Colors.xaml is at index 0 and Styles.xaml is at index 1
            if (dictionaries.Count > 0)
            {
                var newTheme = new System.Windows.ResourceDictionary
                {
                    Source = new System.Uri(value ? "pack://application:,,,/Resources/DarkColors.xaml" : "pack://application:,,,/Resources/Colors.xaml")
                };
                dictionaries[0] = newTheme;
            }
        }
    }
}
