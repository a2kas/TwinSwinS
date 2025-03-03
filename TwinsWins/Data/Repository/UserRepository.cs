using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TwinsWins.Data.Model;
using TwinsWins.Hubs;

namespace TwinsWins.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabseContext _context;

        public UserRepository(DatabseContext context, IHubContext<GameHub> hubContext)
        {
            _context = context;
        }

        public async Task<User> GetUserByWalletAddress(string walletAddress)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Address == walletAddress);
        }

        public async Task Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
