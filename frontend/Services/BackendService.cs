using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ThreatPilot.Frontend.Models;

namespace ThreatPilot.Frontend.Services
{
    /// <summary>
    /// Connects the WPF frontend to the .NET backend using REST + polling.
    /// SignalR Client NuGet was unavailable due to network issues, so we
    /// poll GET /api/Alerts every few seconds to pick up new alerts.
    /// </summary>
    public class BackendService
    {
        private static BackendService? _instance;
        public static BackendService Instance => _instance ??= new BackendService();

        private readonly HttpClient _httpClient;

        private string _baseUrl = "http://localhost:5229";
        private CancellationTokenSource? _pollCts;

        // The last alert count we saw — used to detect new alerts
        private int _lastKnownAlertCount = 0;

        /// <summary>Fired on the UI thread whenever a new alert arrives.</summary>
        public event Action<AlertModel>? OnAlertReceived;

        private BackendService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            _httpClient = new HttpClient(handler);
        }

        public Task InitializeAsync()
        {
            try
            {
                _baseUrl = Environment.GetEnvironmentVariable("BACKEND_URL") ?? _baseUrl;
                _httpClient.BaseAddress = new Uri(_baseUrl);

                // Start polling immediately without auth
                StartPolling();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BackendService] Init failed: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        /// <summary>Fetch all alerts from the backend REST API.</summary>
        public async Task<List<AlertModel>> GetAlertsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Alerts");
                if (response.IsSuccessStatusCode)
                {
                    var alerts = await response.Content.ReadFromJsonAsync<List<AlertModel>>();
                    return alerts ?? new List<AlertModel>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BackendService] Fetch alerts failed: {ex.Message}");
            }

            return new List<AlertModel>();
        }

        /// <summary>Clear all alerts from the backend database.</summary>
        public async Task<bool> ClearAlertsAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync("api/Alerts");
                if (response.IsSuccessStatusCode)
                {
                    _lastKnownAlertCount = 0; // Reset counter
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BackendService] Clear alerts failed: {ex.Message}");
            }
            return false;
        }

        /// <summary>Start background polling every 5 seconds to detect new alerts.</summary>
        private void StartPolling()
        {
            _pollCts = new CancellationTokenSource();
            var token = _pollCts.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(5000, token);

                        var alerts = await GetAlertsAsync();
                        if (alerts.Count > _lastKnownAlertCount)
                        {
                            // New alerts appeared — fire events for the new ones
                            var newAlerts = alerts.GetRange(_lastKnownAlertCount, alerts.Count - _lastKnownAlertCount);
                            _lastKnownAlertCount = alerts.Count;

                            foreach (var alert in newAlerts)
                            {
                                App.Current?.Dispatcher.Invoke(() =>
                                {
                                    OnAlertReceived?.Invoke(alert);
                                });
                            }
                        }
                        else
                        {
                            _lastKnownAlertCount = alerts.Count;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[BackendService] Poll error: {ex.Message}");
                    }
                }
            }, token);
        }

        public void StopPolling()
        {
            _pollCts?.Cancel();
        }


    }
}
