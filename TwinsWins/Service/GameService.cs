using Microsoft.AspNetCore.SignalR;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Timers;
using TwinsWins.Data.Model;
using TwinsWins.Data.Repository;
using TwinsWins.Hubs;
using TwinsWins.Service.Model;

namespace TwinsWins.Services
{
    public class GameService : IGameService, IDisposable
    {
        private List<Cell> _cells;
        private Dictionary<int, int> _imageIdMap;
        private System.Timers.Timer _gameTimer;
        private System.Timers.Timer _countdownTimer;
        private DateTime _gameStartTime;
        private readonly ImageService _imageService;
        private readonly ImageService _paidImageService;
        private readonly IGameRepository _gameRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<GameHub> _hubContext;
        private SynchronizationContext _syncContext;

        public List<Cell> Cells { get; private set; } = new List<Cell>();
        public Cell FirstSelectedCell { get; private set; }
        public Cell SecondSelectedCell { get; private set; }
        public bool IsGameActive { get; private set; }
        public bool IsCountdownActive { get; private set; }
        public int CountdownValue { get; private set; }
        public int TimeRemaining { get; private set; }
        public int Score { get; private set; }
        public bool IsFreeGame { get; private set; }

        public event Action OnGameStateChanged;
        public event Action<int> OnGameEnded;

        public GameService(ImageService imageService,
            ImageService paidImageService,
            IUserRepository userRepository,
            IGameRepository gameRepository,
            IHubContext<GameHub> hubContext,
            IJSRuntime jsRuntime)
        {
            _imageService = imageService;
            _paidImageService = paidImageService;
            _gameRepository = gameRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _syncContext = SynchronizationContext.Current;
        }

        public async Task<List<Game>> GetAvailableGames() 
        {
            return await _gameRepository.GetAvailableGames();
        }

        public async Task<List<Cell>> InitFreeGame()
        {
            IsFreeGame = true;
            var imagePairs = _imageService.GetRandomImagePairs(9);
            return await Task.FromResult(CreateGameCells(imagePairs));
        }

        public async Task<List<Cell>> InitPaidGame(string walletAddress, decimal stake)
        {
            var user = await _userRepository.GetUserByWalletAddress(walletAddress);
            if (user == null)
                throw new Exception("User not found");

            var imagePairs = _paidImageService.GetRandomImagePairs(9);
            var cells = CreateGameCells(imagePairs);

            var gameBody = new GameBody
            {
                Cells = cells,
                ImageIdMap = _imageIdMap
            };

            var newGame = new Game
            {
                OwnerId = user.Id,
                Stake = stake,
                Created = DateTime.UtcNow,
                Body = JsonConvert.SerializeObject(gameBody)
            };

            await _gameRepository.CreateGame(newGame);
            await _hubContext.Clients.All.SendAsync("ReceiveNewGame", newGame);

            Cells = cells;
            IsGameActive = false;
            Score = 0;
            IsFreeGame = false;

            StartCountdown();

            return cells;
        }

        public async Task<List<Cell>> JoinPaidGame(string walletAddress, long gameId)
        {
            var user = await _userRepository.GetUserByWalletAddress(walletAddress);
            var game = await _gameRepository.GetGameById(gameId);
            var gameBody = JsonConvert.DeserializeObject<GameBody>(game.Body);

            _imageIdMap = gameBody.ImageIdMap;
            Cells = gameBody.Cells;

            IsGameActive = false;
            Score = 0;
            IsFreeGame = false;

            StartCountdown();

            return Cells;
        }

        private List<Cell> CreateGameCells(List<ImagePair> imagePairs)
        {
            _cells = new List<Cell>();
            _imageIdMap = new Dictionary<int, int>();

            int id = 1;
            foreach (var imagePair in imagePairs)
            {
                var cell1 = new Cell { Id = id++, ImagePath = imagePair.ImagePath1, IsMatched = false, IsClicked = false };
                var cell2 = new Cell { Id = id++, ImagePath = imagePair.ImagePath2, IsMatched = false, IsClicked = false };

                _cells.Add(cell1);
                _cells.Add(cell2);

                _imageIdMap[cell1.Id] = cell2.Id;
                _imageIdMap[cell2.Id] = cell1.Id;
            }

            _cells = _cells.OrderBy(c => Guid.NewGuid()).ToList();
            Cells = _cells;
            return _cells;
        }

        public void StartCountdown()
        {
            IsCountdownActive = true;
            CountdownValue = 3;

            _countdownTimer = new System.Timers.Timer(1000);
            _countdownTimer.Elapsed += CountdownCallback;
            _countdownTimer.Start();
            OnGameStateChanged?.Invoke();
        }

        private async void CountdownCallback(object sender, ElapsedEventArgs e)
        {
            if (CountdownValue > 1)
                CountdownValue--;
            else
            {
                CountdownValue = 0;
                IsCountdownActive = false;
                _countdownTimer.Stop();
                await StartGame();
            }

            InvokeGameStateChanged();
        }

        public async Task StartGame()
        {
            IsGameActive = true;
            TimeRemaining = 60;
            Score = 0;
            _gameStartTime = DateTime.Now;
            _gameTimer = new System.Timers.Timer(1000);
            _gameTimer.Elapsed += TimerCallback;
            _gameTimer.Start();

            //if (IsFreeGame) 
            //{ 
            //    Cells = await InitFreeGame();
            //}

            InvokeGameStateChanged();
        }

        private void TimerCallback(object sender, ElapsedEventArgs e)
        {
            if (TimeRemaining > 0)
            {
                TimeRemaining--;
                InvokeGameStateChanged();
            }
            else
                EndGame();
        }

        public void EndGame()
        {
            IsGameActive = false;
            _gameTimer?.Stop();
            OnGameEnded?.Invoke(Score);
            InvokeGameStateChanged();
        }

        public async Task<bool> CheckForMatch(int firstCellId, int secondCellId)
        {
            if (_imageIdMap.ContainsKey(firstCellId) && _imageIdMap[firstCellId] == secondCellId)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task CellClicked(Cell cell)
        {
            if (!IsGameActive || cell.IsMatched || cell.IsClicked)
                return;

            cell.IsClicked = true;

            if (FirstSelectedCell == null)
            {
                FirstSelectedCell = cell;
            }
            else if (SecondSelectedCell == null)
            {
                SecondSelectedCell = cell;
                bool isMatch = await CheckForMatch(FirstSelectedCell.Id, SecondSelectedCell.Id);

                if (isMatch)
                {
                    FirstSelectedCell.IsMatched = true;
                    SecondSelectedCell.IsMatched = true;
                    Score += CalculatePoints(true);
                }
                else
                {
                    FirstSelectedCell.IsClicked = false;
                    SecondSelectedCell.IsClicked = false;
                    Score += CalculatePoints(false);
                }

                FirstSelectedCell = null;
                SecondSelectedCell = null;
            }

            if (Cells.All(c => c.IsMatched))
            {
                EndGame();
            }
            else 
            { 
                InvokeGameStateChanged(); 
            }
        }

        private int CalculatePoints(bool isPairMatched)
        {
            var elapsedTime = (DateTime.Now - _gameStartTime).TotalSeconds;
            var timeRatio = elapsedTime / 60.0;
            const int maxPoints = 1000;

            // Reduce points over time
            var points = Math.Floor(maxPoints * (1 - timeRatio));

            return isPairMatched ? Math.Max((int)points, 0) : -100;
        }

        private void InvokeGameStateChanged()
        {
            _syncContext?.Post(_ => OnGameStateChanged?.Invoke(), null);
        }

        public void Dispose()
        {
            _gameTimer?.Stop();
            _gameTimer?.Dispose();
            _countdownTimer?.Stop();
            _countdownTimer?.Dispose();
        }
    }
}
