using Microsoft.AspNetCore.SignalR;
using TwinsWins.Data.Model;

namespace TwinsWins.Hubs
{
    public class GameHub : Hub
    {
        public async Task NewGame(Game game)
        {
            await Clients.All.SendAsync("ReceiveNewGame1", game);
        }
    }
}
