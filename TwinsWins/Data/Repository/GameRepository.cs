using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TwinsWins.Data.Model;
using TwinsWins.Hubs;

namespace TwinsWins.Data.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly DatabseContext _context;

        public GameRepository(DatabseContext context, IHubContext<GameHub> hubContext)
        {
            _context = context;
        }

        public async Task<List<Game>> GetAvailableGames()
        {
            return await _context.Games.ToListAsync();
        }

        public async Task<Game> GetGameById(long gameId)
        {
            return await _context.Games.FindAsync(gameId);
        }

        public async Task CreateGame(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGame(long gameId)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }
    }
}
