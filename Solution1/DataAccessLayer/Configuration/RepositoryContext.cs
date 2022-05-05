using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Configuration
{
    public class RepositoryContext : DbContext
    {
        public DbSet<Weather> CurrentWeathers { get; set; }

        public DbSet<User> Users { get; set; }

        public RepositoryContext(DbContextOptions<RepositoryContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
