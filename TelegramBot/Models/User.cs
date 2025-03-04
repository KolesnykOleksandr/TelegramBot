namespace TelegramBot.Models
{
    public class User
    {
        public required int user_id { get; set; }
        public required string nickname { get; set; }
        public required long chat_id { get; set; }
        public bool isBanned { get; set; }

        public List<WeatherHistory>? weather_history { get; set; }
    }
}
