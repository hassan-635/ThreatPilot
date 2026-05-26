using System.Configuration;
using System.Data;
using System.Windows;

namespace ThreatPilot.Frontend;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Load environment variables from .env file
        string envPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../.env");
        if (System.IO.File.Exists(envPath))
        {
            DotNetEnv.Env.Load(envPath);
        }
        else
        {
            // Fallback for deployed apps
            DotNetEnv.Env.Load();
        }

        // Initialize connection to the backend
        await ThreatPilot.Frontend.Services.BackendService.Instance.InitializeAsync();
    }
}
