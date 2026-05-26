using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ThreatPilot.Backend.Data;
using ThreatPilot.Backend.DTOs;
using ThreatPilot.Backend.Hubs;
using ThreatPilot.Backend.Models;

namespace ThreatPilot.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<AlertHub> _alertHub;

        public IngestController(
            ApplicationDbContext context, 
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHubContext<AlertHub> alertHub)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _alertHub = alertHub;
        }

        [HttpPost]
        public async Task<IActionResult> IngestLogs([FromBody] LogBatchDto batch)
        {
            if (batch.Logs == null || !batch.Logs.Any())
            {
                return BadRequest("No logs provided.");
            }

            // 1. Save raw logs to DB
            var securityEvents = batch.Logs.Select(l => new SecurityEvent
            {
                EventId = Guid.TryParse(l.EventId, out var g) ? g : Guid.NewGuid(),
                Timestamp = l.Timestamp.ToUniversalTime(),
                SourceIp = l.SourceIp,
                UserId = l.UserId,
                EventType = l.EventType,
                TargetResource = l.TargetResource,
                Status = l.Status,
                Metadata = JsonSerializer.Serialize(l.Metadata)
            }).ToList();

            _context.SecurityEvents.AddRange(securityEvents);
            await _context.SaveChangesAsync();

            // 2. Forward to Python Detection Engine
            var client = _httpClientFactory.CreateClient("AiEngine");

            var ingestResponse = await client.PostAsJsonAsync("ingest", batch);
            
            if (!ingestResponse.IsSuccessStatusCode)
            {
                var error = await ingestResponse.Content.ReadAsStringAsync();
                // Log error but don't fail the ingestion
                Console.WriteLine($"Detection Engine Error: {error}");
                return Ok(new { Message = $"Successfully ingested {batch.Logs.Count} logs. Detection failed." });
            }

            var alerts = await ingestResponse.Content.ReadFromJsonAsync<List<AlertDto>>();

            if (alerts != null && alerts.Any())
            {
                // 3. For each alert, generate AI Report
                foreach (var alertDto in alerts)
                {
                    var analysisRequest = new IncidentAnalysisRequestDto { Alert = alertDto };
                    var aiResponse = await client.PostAsJsonAsync("analyze-incident", analysisRequest);
                    
                    if (aiResponse.IsSuccessStatusCode)
                    {
                        var aiReport = await aiResponse.Content.ReadFromJsonAsync<AIReportDto>();
                        
                        // 4. Save fully enriched Alert to DB
                        var newAlert = new Alert
                        {
                            AlertId = Guid.TryParse(alertDto.AlertId, out var ag) ? ag : Guid.NewGuid(),
                            RuleName = alertDto.RuleName,
                            Severity = alertDto.Severity,
                            Timestamp = alertDto.Timestamp.ToUniversalTime(),
                            Description = alertDto.Description,
                            SourceIp = alertDto.SourceIp,
                            UserId = alertDto.UserId,
                            TriggeringLogs = JsonSerializer.Serialize(alertDto.TriggeringLogs),
                            AiSummary = aiReport?.Summary,
                            AiSeverityReason = aiReport?.SeverityReason,
                            AiRecommendedActions = JsonSerializer.Serialize(aiReport?.RecommendedActions),
                            Status = "Open"
                        };

                        _context.Alerts.Add(newAlert);
                        await _context.SaveChangesAsync();

                        // 5. Broadcast to SignalR connected clients
                        await _alertHub.Clients.All.SendAsync("ReceiveAlert", newAlert);
                    }
                }
            }

            return Ok(new { Message = $"Successfully ingested {batch.Logs.Count} logs and processed {alerts?.Count ?? 0} alerts." });
        }


    }
}
