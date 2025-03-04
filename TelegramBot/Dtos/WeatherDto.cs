namespace TelegramBot.Dtos
{
    public class WeatherDto
    {
        public required string City { get; set; }
        public required float Temperature { get; set; }
        public required float FeelsLike { get; set; }
        public required float TempMin { get; set; }
        public required float TempMax { get; set; }
        public required int Pressure { get; set; }
        public required int Humidity { get; set; }
        public required float WindSpeed { get; set; }
        public required int Cloudiness { get; set; }
        public required string WeatherMain { get; set; }
        public required string WeatherDescription { get; set; }
        public required DateTime Timestamp { get; set; }
    }
}
