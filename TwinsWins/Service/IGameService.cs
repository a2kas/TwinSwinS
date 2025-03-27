using TwinsWins.Data.Model;
using TwinsWins.Service.Model;

namespace TwinsWins.Services
{
    public interface IGameService
    {
        List<Cell> Cells { get; }
        bool IsGameActive { get; }
        bool IsCountdownActive { get; }
        int CountdownValue { get; }
        int TimeRemaining { get; }
        int Score { get; }
        bool ShouldShowImages { get; }

        event Action OnGameStateChanged;
        event Action<int> OnGameEnded;

        Task<List<GameLobby>> GetAvailableGames();
        Task<List<Cell>> InitFreeGame();
        Task<List<Cell>> InitPaidGame(string walletAddress, decimal stake);
        Task<List<Cell>> JoinPaidGame(string walletAddress, long gameId);
        void StartCountdown();
        Task StartGame();
        void EndGame();
        Task<bool> CheckForMatch(int firstCellId, int secondCellId);
        Task CellClicked(Cell cell);
    }
}
