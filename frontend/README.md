# ThreatPilot Frontend

Welcome to the `ThreatPilot` frontend application. This is a modern, responsive desktop application built with **Windows Presentation Foundation (WPF)** and **.NET 8**. It serves as the primary SOC (Security Operations Center) dashboard for analysts to monitor, investigate, and respond to cyber threats in real time.

## 🚀 Key Features

- **Real-Time Dashboard**: Monitor system health, active threats, and recent activity streams.
- **Threat Center (Alerts View)**: A comprehensive list of all detected security incidents with filtering by severity (Critical, High, Medium, Low).
- **AI Incident Analysis**: Clicking on any alert opens a detailed incident view populated with AI-generated executive summaries, severity reasoning, and actionable mitigation steps.
- **Modern UI/UX**: Premium dark/light theme, glassmorphism effects, custom control styling, and micro-animations to enhance user experience.
- **MVVM Architecture**: Clean separation of concerns using the Model-View-ViewModel pattern powered by the CommunityToolkit.Mvvm library.

## 📂 Project Structure

- `Views/` — XAML files defining the user interface (e.g., `DashboardView.xaml`, `AlertsView.xaml`, `IncidentView.xaml`).
- `ViewModels/` — C# classes handling presentation logic and state (e.g., `MainViewModel.cs`, `AlertsViewModel.cs`).
- `Models/` — Data structures representing alerts, system metrics, and logs (e.g., `AlertModel.cs`).
- `Services/` — Backend communication logic (e.g., `BackendService.cs` for REST API polling).
- `Resources/` — Reusable XAML styles, colors, value converters, and themes.
- `App.xaml` — Application entry point and global resource dictionary definition.

## 🛠️ Technology Stack

- **Framework**: .NET 8 WPF (Windows Presentation Foundation)
- **Architecture**: MVVM (Model-View-ViewModel)
- **Libraries**:
  - `CommunityToolkit.Mvvm` (for ObservableObjects and RelayCommands)
  - `System.Text.Json` (for API serialization)
- **API Communication**: `HttpClient` (REST)

## ⚙️ How to Setup & Run

1. Make sure you have the **.NET 8 SDK** installed.
2. Ensure the ThreatPilot **Backend** and **AI Engine** are running locally.
3. Open a terminal, navigate to the `frontend` folder, and run:

```powershell
cd f:\ThreatPilot\frontend
dotnet run
```

This will compile the XAML and C# code, and launch the ThreatPilot WPF application. You can also open `ThreatPilot.Frontend.csproj` in Visual Studio 2022 and click "Start".
