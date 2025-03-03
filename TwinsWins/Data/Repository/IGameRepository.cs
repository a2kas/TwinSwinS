using TwinsWins.Data.Model;

namespace TwinsWins.Data.Repository
{
    public interface IGameRepository
    {
        Task<List<Game>> GetAvailableGames();
        Task<Game> GetGameById(long gameId);
        Task CreateGame(Game game);
        Task DeleteGame(long gameId);
    }
}
