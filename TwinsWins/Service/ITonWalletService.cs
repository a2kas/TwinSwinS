using TonSdk.Connect;

namespace TwinsWins.Services
{
    public interface ITonWalletService
    {
        event Action<Wallet>? OnWalletConnected;
        event Action<string>? OnErrorOccurred;

        bool IsConnected { get; }
        string WalletAddress { get; }
        WalletConfig[]? GetWallets();
        Task<string> ConnectWalletAsync(WalletConfig walletConfig);
        Task<bool> RestoreConnectionAsync();
        Task DisconnectAsync();

        Task<string> SubmitScoreAsync(uint score, string? referralAddress = null, decimal amount = 0.1m);
        Task<string> InitializeGameAsync(string developerAddress, decimal amount = 0.1m);
        Task<GameStatus> GetGameStatusAsync();
        Task<(bool CanSubmit, string? Reason)> CanSubmitScoreAsync();
        Task<bool> CheckGameTimeoutAsync();
    }
}
