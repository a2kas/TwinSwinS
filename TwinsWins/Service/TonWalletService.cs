using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using TonSdk.Client;
using TonSdk.Connect;
using TonSdk.Core;
using TonSdk.Core.Boc;
using TwinsWins.Data.Model;
using TwinsWins.Data.Repository;

namespace TwinsWins.Services
{
    public class TonWalletService : ITonWalletService
    {
        private readonly TonConnect _tonConnect;
        private readonly TonClient _tonClient;
        private WalletConfig[]? _walletsConfig;
        private readonly IUserRepository _userRepository;
        private const string ManifestUrl = "https://raw.githubusercontent.com/ton-community/tutorials/main/03-client/test/public/tonconnect-manifest.json";
        private const string ApiKey = "5b3c1bf0c7f7b1995efd6b71e537330f3e8d23f2625c8f8a4a0db66bb853216d";

        // Game contract constants
        private const string ContractAddress = "EQB4SrBdve8cDp7x_0pGthk-llNVyCZMdHBiJeWQEii1LFkK";
        private const int INITIALIZE_GAME_OP = 1;
        private const int SUBMIT_SCORE_OP = 2;
        private const int ADD_BONUS_CODE_OP = 3;
        private const int REDEEM_BONUS_CODE_OP = 4;
        private const int REMOVE_BONUS_CODE_OP = 5;
        private const long MIN_TON_VALUE = 100000000;

        public event Action<TonSdk.Connect.Wallet>? OnWalletConnected;
        public event Action<string>? OnErrorOccurred;

        public TonWalletService(IUserRepository userRepository) 
        {
            _tonConnect = new TonConnect(new TonConnectOptions { ManifestUrl = ManifestUrl  });
            _tonConnect.OnStatusChange(OnStatusChange, OnErrorChange);
            _tonClient = new TonClient(TonClientType.HTTP_TONCENTERAPIV2, new HttpParameters
            {
                Endpoint = "https://testnet.toncenter.com/api/v2/jsonRPC",
                ApiKey = ApiKey
            });

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

        public async Task<decimal> GetWalletBalanceAsync(Address? Address)
        {
            try
            {
                var accountState = await _tonClient.GetBalance(Address);
                return accountState.ToDecimal();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private BigInteger HashBonusCode(string bonusCode)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(bonusCode));
                return new BigInteger(hashBytes);
            }
        }

        public async Task<string> AddBonusCodeAsync(string bonusCode, decimal bonusAmount, decimal amount = 0.05m)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Wallet is not connected");
            }

            var amountCoins = new Coins(amount);
            var bonusAmountCoins = new Coins(bonusAmount);

            try
            {
                // Hash the bonus code to a 256-bit value
                BigInteger bonusCodeHash = HashBonusCode(bonusCode);

                var body = new CellBuilder()
                    .StoreUInt(ADD_BONUS_CODE_OP, 32)
                    .StoreUInt(bonusCodeHash, 256)
                    .StoreCoins(bonusAmountCoins)
                    .Build();

                var message = new Message[] {
                    new Message(
                        new Address(ContractAddress),
                        amountCoins,
                        null,
                        body
                    )
                };

                var request = new SendTransactionRequest(
                    message,
                    DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds()
                );

                var result = await _tonConnect.SendTransaction(request);
                return result.Value.Boc.ToString();
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke($"Failed to add bonus code: {ex.Message}");
                throw;
            }
        }

        public async Task<string> RedeemBonusCodeAsync(string bonusCode, decimal amount = 0.05m)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Wallet is not connected");
            }

            var amountCoins = new Coins(amount);

            try
            {
                // Hash the bonus code to a 256-bit value
                BigInteger bonusCodeHash = HashBonusCode(bonusCode);

                var body = new CellBuilder()
                    .StoreUInt(REDEEM_BONUS_CODE_OP, 32)
                    .StoreUInt(bonusCodeHash, 256)
                    .Build();

                var message = new Message[] {
                    new Message(
                        new Address(ContractAddress),
                        amountCoins,
                        null,
                        body
                    )
                };

                var request = new SendTransactionRequest(
                    message,
                    DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds()
                );

                var result = await _tonConnect.SendTransaction(request);
                return result.Value.Boc.ToString();
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke($"Failed to redeem bonus code: {ex.Message}");
                throw;
            }
        }

        public async Task<string> RemoveBonusCodeAsync(string bonusCode, decimal amount = 0.05m)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Wallet is not connected");
            }

            var amountCoins = new Coins(amount);

            try
            {
                // Hash the bonus code to a 256-bit value
                BigInteger bonusCodeHash = HashBonusCode(bonusCode);

                var body = new CellBuilder()
                    .StoreUInt(REMOVE_BONUS_CODE_OP, 32)
                    .StoreUInt(bonusCodeHash, 256)
                    .Build();

                var message = new Message[] {
                    new Message(
                        new Address(ContractAddress),
                        amountCoins,
                        null,
                        body
                    )
                };

                var request = new SendTransactionRequest(
                    message,
                    DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds()
                );

                var result = await _tonConnect.SendTransaction(request);
                return result.Value.Boc.ToString();
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke($"Failed to remove bonus code: {ex.Message}");
                throw;
            }
        }


        public async Task<string> SubmitScoreAsync(uint score, string? referralAddress = null, decimal amount = 0.1m)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Wallet is not connected");
            }

              var amountCoins = new Coins(amount);
            if (decimal.Parse(amountCoins.ToNano()) < MIN_TON_VALUE)
            {
                throw new ArgumentException($"Minimum amount is 0.1 TON (got {amount} TON)");
            }

            try
            {
                var cellBuilder = new CellBuilder()
                    .StoreUInt(SUBMIT_SCORE_OP, 32)
                    .StoreUInt(score, 32);

                Address referral;
                if (!string.IsNullOrEmpty(referralAddress))
                {
                    referral = new Address(referralAddress);
                }
                else
                {
                    referral = new Address(0, new byte[32]);
                }
                cellBuilder.StoreAddress(referral);

                var body = cellBuilder.Build();

                var message = new Message[] {
                    new Message(
                        new Address(ContractAddress),
                        amountCoins,
                        null,
                        body
                    )
                };

                var request = new SendTransactionRequest(
                    message,
                    DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds()
                );

                var result = await _tonConnect.SendTransaction(request);
                return result.Value.Boc.ToString();
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke($"Failed to submit score: {ex.Message}");
                throw;
            }
        }

        public async Task<string> InitializeGameAsync(string developerAddress, decimal amount = 0.1m)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Wallet is not connected");
            }

            var amountCoins = new Coins(amount);
            if (decimal.Parse(amountCoins.ToNano()) < MIN_TON_VALUE)
            {
                throw new ArgumentException($"Minimum amount is 0.1 TON (got {amount} TON)");
            }

            try
            {
                var body = new CellBuilder()
                    .StoreUInt(INITIALIZE_GAME_OP, 32)
                    .StoreAddress(new Address(developerAddress))
                    .Build();

                var message = new Message[] {
                    new Message(
                        new Address(ContractAddress),
                        amountCoins,
                        null,
                        body
                    )
                };

                var request = new SendTransactionRequest(
                    message,
                    DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds()
                );

                var result = await _tonConnect.SendTransaction(request);
                return result.Value.Boc.ToString();
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke($"Failed to initialize game: {ex.Message}");
                throw;
            }
        }

        public async Task<GameStatus> GetGameStatusAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ContractAddress))
                {
                    throw new InvalidOperationException("Contract address is not set");
                }

                // Get contract data
                var contractAddr = new Address(ContractAddress);
                var addressInfo = await _tonClient.GetAddressInformation(contractAddr);

                // If contract is not deployed or has no data
                if (addressInfo.Value.State != AccountState.Active || addressInfo.Value.Data == null)
                {
                    return new GameStatus
                    {
                        IsGameInitialized = false,
                        Player1Address = string.Empty,
                        Player1Score = 0,
                        Player2Address = string.Empty,
                        Player2Score = 0,
                        PrizePool = addressInfo.Value.Balance.ToDecimal(),
                        SubmissionTime = 0,
                        Error = "Contract not deployed or has no data"
                    };
                }

                // Parse contract data
                try
                {
                    // Extract data from the storage cell
                    var cellSlice = addressInfo.Value.Data.Parse();

                    // Parse according to storage format in twin_swin_s.fc:
                    // player1_address:MsgAddressInt player1_score:int player1_ref:MsgAddressInt 
                    // player2_address:MsgAddressInt player2_score:int player2_ref:MsgAddressInt
                    // developer_address:MsgAddressInt submission_time:int prize_pool:int

                    // Parse player1 data first
                    var player1Address = cellSlice.LoadAddress()?.ToString() ?? string.Empty;
                    var player1Score = (int)cellSlice.LoadInt(32);
                    var player1RefAddress = cellSlice.LoadAddress()?.ToString() ?? string.Empty;

                    // Parse player2 data
                    var player2Address = cellSlice.LoadAddress()?.ToString() ?? string.Empty;
                    var player2Score = (int)cellSlice.LoadInt(32);
                    var player2RefAddress = cellSlice.LoadAddress()?.ToString() ?? string.Empty;

                    // Parse developer data and other fields
                    var developerAddress = cellSlice.LoadAddress()?.ToString() ?? string.Empty;
                    var submissionTime = (long)cellSlice.LoadUInt(64);
                    var prizePool = cellSlice.LoadCoins().ToDecimal();

                    var isPlayerOneAddressNonEmpty = player1Address != "0:0000000000000000000000000000000000000000000000000000000000000000"
                                                && player1Address != string.Empty;

                    return new GameStatus
                    {
                        IsGameInitialized = isPlayerOneAddressNonEmpty,
                        Player1Address = player1Address,
                        Player1Score = player1Score,
                        Player1RefAddress = player1RefAddress,
                        Player2Address = player2Address,
                        Player2Score = player2Score,
                        Player2RefAddress = player2RefAddress,
                        DeveloperAddress = developerAddress,
                        PrizePool = prizePool,
                        SubmissionTime = submissionTime
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing contract data: {ex.Message}");
                    return new GameStatus
                    {
                        IsGameInitialized = false,
                        Player1Address = string.Empty,
                        Player1Score = 0,
                        Player2Address = string.Empty,
                        Player2Score = 0,
                        PrizePool = addressInfo.Value.Balance.ToDecimal(),
                        SubmissionTime = 0,
                        Error = $"Error parsing contract data: {ex.Message}"
                    };
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke($"Failed to get game status: {ex.Message}");
                return new GameStatus
                {
                    IsGameInitialized = false,
                    Player1Address = string.Empty,
                    Player1Score = 0,
                    Player2Address = string.Empty,
                    Player2Score = 0,
                    PrizePool = 0,
                    SubmissionTime = 0,
                    Error = ex.Message
                };
            }
        }

        public async Task<(bool CanSubmit, string? Reason)> CanSubmitScoreAsync()
        {
            if (!IsConnected)
            {
                return (false, "Wallet is not connected");
            }

            var status = await GetGameStatusAsync();
            var currentAddress = WalletAddress;

            if (!status.IsGameInitialized)
            {
                return (true, null);
            }

            if (status.IsGameComplete)
            {
                return (false, "Game is already complete");
            }

            if (status.Player1Address.Equals(currentAddress, StringComparison.OrdinalIgnoreCase))
            {
                return (false, "You have already submitted your score");
            }

            return (true, null);
        }

        public async Task<bool> CheckGameTimeoutAsync()
        {
            var status = await GetGameStatusAsync();


            if (status.IsGameInitialized &&
                string.IsNullOrEmpty(status.Player2Address) ||
                status.Player2Address == "0:0000000000000000000000000000000000000000000000000000000000000000")
            {
                var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return (currentTime - status.SubmissionTime) > 86400;
            }

            return false;
        }


        /// <summary>
        /// Helper to convert TON decimal to nano TON (smallest unit)
        /// </summary>
        private long ConvertToNanoTon(decimal ton)
        {
            return (long)(ton * 1_000_000_000m);
        }


        private async void OnStatusChange(TonSdk.Connect.Wallet wallet)
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
    public class GameStatus
    {
        public bool IsGameInitialized { get; set; }
        public string Player1Address { get; set; } = string.Empty;
        public int Player1Score { get; set; }
        public string Player1RefAddress { get; set; } = string.Empty;
        public string Player2Address { get; set; } = string.Empty;
        public int Player2Score { get; set; }
        public string Player2RefAddress { get; set; } = string.Empty;
        public string DeveloperAddress { get; set; } = string.Empty;
        public decimal PrizePool { get; set; }
        public long SubmissionTime { get; set; }
        public string? Error { get; set; }

        public bool IsGameComplete => !string.IsNullOrEmpty(Player2Address) &&
                                      Player2Address != "0:0000000000000000000000000000000000000000000000000000000000000000";

        public bool HasTimedOut
        {
            get
            {
                if (!IsGameInitialized || IsGameComplete) return false;
                var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return (currentTime - SubmissionTime) > 86400; // 24 hours in seconds
            }
        }

        public string GetWinnerAddress()
        {
            if (!IsGameComplete) return string.Empty;

            if (Player1Score == Player2Score)
            {
                return "Tie - prize split between players";
            }

            return Player1Score > Player2Score ? Player1Address : Player2Address;
        }
    }
}