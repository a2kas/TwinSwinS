using TonSdk.Connect;
using TwinsWins.Data;
using TwinsWins.Data.Model;
using TwinsWins.Data.Repository;

namespace TwinsWins.Services
{
    public class TonWalletService : ITonWalletService
    {
        private readonly TonConnect? _tonConnect;
        private WalletConfig[]? _walletsConfig;
        private readonly IUserRepository _userRepository;
        private const string ManifestUrl = "https://raw.githubusercontent.com/ton-community/tutorials/main/03-client/test/public/tonconnect-manifest.json";
        
        public event Action<Wallet>? OnWalletConnected;
        public event Action<string>? OnErrorOccurred;

        public TonWalletService(IUserRepository userRepository) 
        {
            _tonConnect = new TonConnect(new TonConnectOptions { ManifestUrl = ManifestUrl  });
            _tonConnect.OnStatusChange(OnStatusChange, OnErrorChange);
            _userRepository = userRepository;
        }

        public bool IsConnected => _tonConnect.IsConnected;
        public string WalletAddress => _tonConnect.Account?.Address?.ToString() ?? string.Empty;

        public WalletConfig[]? GetWallets()
        {
            return _walletsConfig ?? _tonConnect.GetWallets();
        }

        public async Task<string> ConnectWalletAsync(WalletConfig walletConfig)
        {
            return await _tonConnect.Connect(walletConfig);
        }

        public async Task<bool> RestoreConnectionAsync()
        {
            return await _tonConnect.RestoreConnection();
        }

        public async Task DisconnectAsync()
        {
           await _tonConnect.Disconnect();
        }

        private async void OnStatusChange(Wallet wallet)
        {
            if (!string.IsNullOrEmpty(wallet.Account?.Address?.ToString()))
            {
                var user = await _userRepository.GetUserByWalletAddress(wallet.Account?.Address.ToString());
                if (user == null)
                {
                    await _userRepository.Create(new User
                    {
                        Address = wallet.Account?.Address.ToString(),
                        Created = DateTime.UtcNow
                    });
                }
            }

            OnWalletConnected?.Invoke(wallet);
        }

        private void OnErrorChange(string error)
        {
            OnErrorOccurred?.Invoke(error);
        }
    }
}