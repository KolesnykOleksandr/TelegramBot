namespace TelegramBot.Application.Interfaces
{
    public interface IWeatherRepository
    {
        Task<object> GetWeatherAsync(string city, string nickname, long chat_id);
    }
}
