using Microsoft.EntityFrameworkCore;
using TwinsWins.Data.Model;

namespace TwinsWins.Data
{
    public class DatabseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }

        public DatabseContext(DbContextOptions<DatabseContext> options) : base(options)
        { 
        }
    }
}
