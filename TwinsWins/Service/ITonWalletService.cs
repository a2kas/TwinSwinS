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
    }
}
