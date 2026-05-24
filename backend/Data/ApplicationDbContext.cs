using Microsoft.EntityFrameworkCore;
using ThreatPilot.Backend.Models;

namespace ThreatPilot.Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SecurityEvent> SecurityEvents { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure PostgreSQL unique constraints and indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Alert>()
                .HasIndex(a => a.Timestamp);

            modelBuilder.Entity<SecurityEvent>()
                .HasIndex(s => s.Timestamp);
        }
    }
}
