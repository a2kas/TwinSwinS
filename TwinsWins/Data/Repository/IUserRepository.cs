using TwinsWins.Data.Model;

namespace TwinsWins.Data.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserByWalletAddress(string walletAddress);
        Task Create(User user);
    }
}
