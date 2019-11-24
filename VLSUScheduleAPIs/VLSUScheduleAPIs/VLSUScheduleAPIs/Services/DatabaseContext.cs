using Microsoft.EntityFrameworkCore;
using Commonlibrary.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace VLSUScheduleAPIs.Services
{
    public class DatabaseContext : DbContext
    {
        private readonly ILogger<DatabaseContext> logger;
        private readonly IConfiguration configuration;

        public DatabaseContext(DbContextOptions options, ILogger<DatabaseContext> logger, IConfiguration configuration) : base(options)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public DbSet<Auditory> Auditories { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<IllCard> IllCards { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            logger.LogInformation("Init db");
            logger.LogInformation($"Connection string {configuration.GetConnectionString("DefaultConnection")}");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Teacher>();
            modelBuilder.Entity<Schedule>().Ignore(x => x.Teacher);
        }
    }
}