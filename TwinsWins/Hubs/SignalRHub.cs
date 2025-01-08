namespace TwinsWins.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using TwinsWins.Data;

    public class SignalRHub : Hub
    {
        public async Task NewGame()
        {
            // Generate new game data
            var newGame = new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = new Random().Next(-20, 55),
                Summary = "New Game"
            };

            // Broadcast the new game data to all connected clients
            await Clients.All.SendAsync("ReceiveNewGame", newGame);
        }
    }

}
