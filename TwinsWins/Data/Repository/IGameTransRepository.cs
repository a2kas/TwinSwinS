using System.Threading.Tasks;
using TwinsWins.Data.Model;

namespace TwinsWins.Data.Repository
{
    public interface IGameTransRepository
    {
        Task<GameTransaction> GetByGameId(long gameId);
        Task CreateGameTransaction(GameTransaction trans);
        Task SetOwnerScore(long gameId, int score);
        Task SetOpponent(long gameId, long opponentId);
        Task SetOpponentScore(long gameId, int score);
    }
}
