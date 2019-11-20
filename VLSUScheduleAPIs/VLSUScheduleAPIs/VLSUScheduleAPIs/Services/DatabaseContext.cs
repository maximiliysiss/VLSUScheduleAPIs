using Microsoft.EntityFrameworkCore;
using Commonlibrary.Models;

namespace VLSUScheduleAPIs.Services
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Auditory> Auditories { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<IllCard> IllCards { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Teacher>();
            modelBuilder.Entity<Schedule>().Ignore(x=>x.Teacher);
        }
    }
}