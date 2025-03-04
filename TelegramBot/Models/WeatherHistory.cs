namespace TelegramBot.Models
{
    public class WeatherHistory
    {
        public int WeatherHistory_Id { get; set; }
        public required int User_Id { get; set; }
        public required string City { get; set; }
        public required float Temperature { get; set; }
        public required float Feels_Like { get; set; }
        public required float Temp_Min { get; set; }
        public required float Temp_Max { get; set; }
        public required int Pressure { get; set; }
        public required int Humidity { get; set; }
        public required float Wind_Speed { get; set; }
        public required int Cloudiness { get; set; }
        public required string Weather_Main { get; set; }
        public required string Weather_Description { get; set; }
        public required int Timestamp { get; set; }
        public required User User { get; set; }
    }

}
