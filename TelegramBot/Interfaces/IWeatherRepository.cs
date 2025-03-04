using TelegramBot.Dtos;
using TelegramBot.Models;

namespace TelegramBot.Interfaces
{
    public interface IWeatherRepository
    {
        Task<object> GetWeatherAsync(string city, string nickname, long chat_id);
    }
}
