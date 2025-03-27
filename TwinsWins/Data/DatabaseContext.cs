using Microsoft.EntityFrameworkCore;
using TwinsWins.Data.Model;

namespace TwinsWins.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<GameLobby> LobbyGames { get; set; }
        public DbSet<GameTransaction> GameTransactions { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { 
        }
    }
}
