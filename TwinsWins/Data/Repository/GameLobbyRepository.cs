using Microsoft.EntityFrameworkCore;
using TwinsWins.Data.Model;

namespace TwinsWins.Data.Repository
{
    public class GameLobbyRepository : BaseRepository, IGameLobbyRepository
    {

        public GameLobbyRepository(IDbContextFactory<DatabaseContext> dbContextFactory) : base(dbContextFactory)
        {
        }

        public async Task<List<GameLobby>> GetAvailableGames()
        {
            using var context = Context;
            return await context.LobbyGames.ToListAsync();
        }

        public async Task<GameLobby> GetById(long gameId)
        {
            using var context = Context;
            return await context.LobbyGames.FindAsync(gameId);
        }

        public async Task<GameLobby> CreateLobbyGame(GameLobby game)
        {
            using var context = Context;
            context.LobbyGames.Add(game);
            await context.SaveChangesAsync();
            return game;
        }

        public async Task DeleteLobbyGame(long gameId)
        {
            using var context = Context;
            var game = await context.LobbyGames.FindAsync(gameId);
            if (game != null)
            {
                context.LobbyGames.Remove(game);
                await context.SaveChangesAsync();
            }
        }
    }
}
