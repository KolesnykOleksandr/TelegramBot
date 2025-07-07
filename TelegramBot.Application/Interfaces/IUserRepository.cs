using TelegramBot.Application.Models;

namespace TelegramBot.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>?> GetUsers();
        Task<User?> GetUser(int user_id);
        Task BanUser(int user_id);
        Task UnbanUser(int user_id);
    }
}
