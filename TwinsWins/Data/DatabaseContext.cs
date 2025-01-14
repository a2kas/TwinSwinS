using Microsoft.EntityFrameworkCore;

namespace TwinsWins.Data
{
    public class DatabseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DatabseContext(DbContextOptions<DatabseContext> options) : base(options)
        {
        }
    }
}
