using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThreatPilot.Backend.Data;
using ThreatPilot.Backend.Models;

namespace ThreatPilot.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AlertsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlertsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Alerts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlerts()
        {
            return await _context.Alerts.OrderByDescending(a => a.Timestamp).ToListAsync();
        }

        // GET: api/Alerts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Alert>> GetAlert(Guid id)
        {
            var alert = await _context.Alerts.FindAsync(id);

            if (alert == null)
            {
                return NotFound();
            }

            return alert;
        }

        // POST: api/Alerts
        // Usually called by the AI engine or Detection logic to save a new alert
        [HttpPost]
        public async Task<ActionResult<Alert>> PostAlert(Alert alert)
        {
            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlert), new { id = alert.AlertId }, alert);
        }

        // DELETE: api/Alerts
        [HttpDelete]
        public async Task<IActionResult> ClearAllAlerts()
        {
            var alerts = await _context.Alerts.ToListAsync();
            _context.Alerts.RemoveRange(alerts);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
