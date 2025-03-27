using Microsoft.EntityFrameworkCore;
using TwinsWins.Data.Model;

namespace TwinsWins.Data.Repository
{
    public class GameTransRepository : BaseRepository, IGameTransRepository
    {
        public GameTransRepository(IDbContextFactory<DatabaseContext> dbContextFactory) : base(dbContextFactory)
        {
        }

        public async Task<GameTransaction> GetByGameId(long gameId)
        {
            using var context = Context;
            return await context.GameTransactions.FindAsync(gameId);
        }

        public async Task CreateGameTransaction(GameTransaction trans)
        {
            using var context = Context;
            context.GameTransactions.Add(trans);
            await context.SaveChangesAsync();
        }

        public async Task SetOwnerScore(long gameId, int score)
        {
            using var context = Context;
            var trans = await context.GameTransactions.FindAsync(gameId);
            if (trans != null)
            {
                trans.OwnerScore = score;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetOpponent(long gameId, long opponentId)
        {
            using var context = Context;
            var trans = await context.GameTransactions.FindAsync(gameId);
            if (trans != null)
            {
                trans.OpponentId = opponentId;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetOpponentScore(long gameId, int score)
        {
            using var context = Context;
            var trans = await context.GameTransactions.FindAsync(gameId);
            if (trans != null)
            {
                trans.OpponentScore = score;
                await context.SaveChangesAsync();
            }
        }
    }
}
