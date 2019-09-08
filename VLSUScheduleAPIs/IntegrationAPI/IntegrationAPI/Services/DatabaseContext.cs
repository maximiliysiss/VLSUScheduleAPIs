using Microsoft.EntityFrameworkCore;

namespace IntegrationAPI.Services
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
    }
}