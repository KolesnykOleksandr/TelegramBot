using Dapper;
using System.Data.SqlClient;
using System.Text.Json;
using TelegramBot.Dtos;
using TelegramBot.Interfaces;
using TelegramBot.Models;

namespace TelegramBot.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly IConfiguration _configuration;

        public WeatherRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<object> GetWeatherAsync(string city, string nickname, long chat_id)
        {
            SqlConnection connection = GetConnection();
            
            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE nickname = @nickname",
                new { nickname }
            );

            if (user == null)
            {
                await connection.ExecuteAsync(
                    "INSERT INTO Users (nickname, isBanned, chat_id) VALUES (@nickname, 0, @chat_id)",
                    new { nickname, chat_id }
                );

                user = await connection.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM Users WHERE nickname = @nickname",
                    new { nickname }
                );
            }


            if (user.isBanned)
            {
                return "Ви образили бота😢 й він тепер не хоче розмовляти(";
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["ApiSettings:RequestUri"] + $"{city}/UK"),
                Headers =
                {
                    { "x-rapidapi-key", _configuration["ApiSettings:RapidApiKey"] },
                    { "x-rapidapi-host", _configuration["ApiSettings:RapidApiHost"] },
                },
            };
            var response = await client.SendAsync(request);


            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(body);
            var root = jsonDoc.RootElement;

            var codProperty = root.GetProperty("cod");
            if (codProperty.ValueKind == JsonValueKind.String)
            {
                return "Місто не було знайдено🙈";
            }
            var weatherHistory = new WeatherHistory
            {
                User_Id = user.user_id,
                City = root.GetProperty("name").GetString() ?? "Unknown",
                Temperature = ConvertFahrenheitToCelsius(root.GetProperty("main").GetProperty("temp").GetSingle()),
                Feels_Like = ConvertFahrenheitToCelsius(root.GetProperty("main").GetProperty("feels_like").GetSingle()),
                Temp_Min = ConvertFahrenheitToCelsius(root.GetProperty("main").GetProperty("temp_min").GetSingle()),
                Temp_Max = ConvertFahrenheitToCelsius(root.GetProperty("main").GetProperty("temp_max").GetSingle()),
                Pressure = root.GetProperty("main").GetProperty("pressure").GetInt32(),
                Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                Wind_Speed = root.GetProperty("wind").GetProperty("speed").GetSingle(),
                Cloudiness = root.GetProperty("clouds").GetProperty("all").GetInt32(),
                Weather_Main = root.GetProperty("weather")[0].GetProperty("main").GetString() ?? "Unknown",
                Weather_Description = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? "Unknown",
                Timestamp = root.GetProperty("dt").GetInt32(),
                User = user
            };

            await connection.ExecuteAsync(
                $"Insert into WeatherHistory(user_id, city, temperature, feels_like, temp_min, temp_max, pressure," +
                $"humidity, wind_speed, cloudiness, weather_main, weather_description, timestamp) " +
                $"Values (@user_id, @city, @temperature, @feels_like, @temp_min, @temp_max, @pressure," +
                $" @humidity, @wind_speed, @cloudiness, @weather_main, @weather_description, @timestamp)", weatherHistory);




            return new WeatherDto
            {
                City = weatherHistory.City,
                Cloudiness = weatherHistory.Cloudiness,
                FeelsLike = weatherHistory.Feels_Like,
                WeatherDescription = weatherHistory.Weather_Description,
                Humidity = weatherHistory.Humidity,
                Pressure = weatherHistory.Pressure,
                WindSpeed = weatherHistory.Wind_Speed,
                Temperature = weatherHistory.Temperature,
                TempMax = weatherHistory.Temp_Max,
                TempMin = weatherHistory.Temp_Min,
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(weatherHistory.Timestamp).UtcDateTime,
                WeatherMain = weatherHistory.Weather_Main
            };

        }

        private float ConvertFahrenheitToCelsius(float farenheit)
        {
            float result = (farenheit - 32) / 1.8f;

            result = (float)Math.Round(result, 1);

            return result;

        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

    }
}
