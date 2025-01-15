namespace TwinsWins.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using TwinsWins.Data;

    public class SignalRHub : Hub
    {
        public async Task NewGame()
        {
            // Generate new game data
            var newGame = new Game
            {
                OwnerId = 1,
                Stake = 11,
                Created = DateTime.Now
            };

           await Clients.All.SendAsync("ReceiveNewGame", newGame);
        }
    }

}
