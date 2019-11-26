using Microsoft.EntityFrameworkCore;
using Commonlibrary.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace AuthAPI.Services
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

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            logger.LogInformation("Init dbContext");
            logger.LogInformation($"Connection string: {configuration.GetConnectionString("DefaultConnection")}");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Group>();
            modelBuilder.Ignore<IllCard>();

            modelBuilder.Entity<User>().HasData(
                new User { ID = 1, Login = "VlsuScheduleAPI", UserType = UserType.Service, Password = CryptService.CreateMD5("VlsuSchedule") },
                new User { ID = 2, Login = "IntegrationAPI", UserType = UserType.Service, Password = CryptService.CreateMD5("IntegrationAPI") },
                new User { ID = 3, Login = "AuthAPI", UserType = UserType.Service, Password = CryptService.CreateMD5("AuthAPI") }
            );
        }
    }
}
