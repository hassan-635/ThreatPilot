using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThreatPilot.Backend.Data;
using ThreatPilot.Backend.Models;

namespace ThreatPilot.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IngestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class LogBatch
        {
            public List<SecurityEvent> Logs { get; set; } = new();
        }

        [HttpPost]
        public async Task<IActionResult> IngestLogs([FromBody] LogBatch batch)
        {
            if (batch.Logs == null || !batch.Logs.Any())
            {
                return BadRequest("No logs provided.");
            }

            // Save raw logs to the database for persistence
            _context.SecurityEvents.AddRange(batch.Logs);
            await _context.SaveChangesAsync();

            // Note: In the future, this is where we will forward the logs to the Python AI engine for detection.
            // For now, we just acknowledge receipt and persistence.
            return Ok(new { Message = $"Successfully ingested {batch.Logs.Count} logs." });
        }
    }
}
