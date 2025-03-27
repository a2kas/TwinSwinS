using TwinsWins.Data.Model;

namespace TwinsWins.Data.Repository
{
    public interface IGameLobbyRepository
    {
        Task<List<GameLobby>> GetAvailableGames();
        Task<GameLobby> GetById(long gameId);
        Task<GameLobby> CreateLobbyGame(GameLobby game);
        Task DeleteLobbyGame(long gameId);
    }
}
