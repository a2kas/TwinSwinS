using Microsoft.EntityFrameworkCore;

namespace TwinsWins.Data.Repository
{
    public class BaseRepository
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public BaseRepository(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected DatabaseContext Context => _dbContextFactory.CreateDbContext();
    }
}
