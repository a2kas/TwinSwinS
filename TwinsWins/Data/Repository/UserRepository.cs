using Microsoft.EntityFrameworkCore;
using TwinsWins.Data.Model;

namespace TwinsWins.Data.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {

        public UserRepository(IDbContextFactory<DatabaseContext> dbContextFactory) : base(dbContextFactory)
        {
        }

        public async Task<User> GetUserByWalletAddress(string walletAddress)
        {
            using var context = Context;
            return await context.Users.FirstOrDefaultAsync(u => u.Address == walletAddress);
        }

        public async Task Create(User user)
        {
            using var context = Context;
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}
