using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Configuration
{
    public class RepositoryContext : DbContext
    {
        public DbSet<Weather> CurrentWeathers { get; set; } = null!;
        public RepositoryContext(DbContextOptions<RepositoryContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
