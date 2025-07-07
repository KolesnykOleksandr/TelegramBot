using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using TelegramBot.Application.Dtos;
using TelegramBot.Application.Interfaces;
using TelegramBot.Application.JsonResponses;
using TelegramBot.Application.Models;

namespace TelegramBot.Infrastructure.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IWeatherApiService _weatherApiService;

        public WeatherRepository(
            IConfiguration configuration,
            IWeatherApiService weatherApiService)
        {
            _configuration = configuration;
            _weatherApiService = weatherApiService;
        }

        public async Task<object> GetWeatherAsync(string city, string nickname, long chat_id)
        {
            using var connection = GetConnection();

            var user = await GetOrCreateUserAsync(connection, nickname, chat_id);

            if (user.isBanned)
            {
                return "Ви образили бота😢 й він тепер не хоче розмовляти(";
            }

            WeatherDto weatherData;
            try
            {
                weatherData = (await _weatherApiService.GetWeatherAsync(city)).ToWeatherDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання погоди: {ex.Message}");
                return "Сталася помилка при отриманні погоди";
            }

            WeatherHistory weatherHistory = new WeatherHistory
            {
                User_Id = user.user_id,
                City = weatherData.City?? "Unknown",
                Temperature = weatherData.Temperature,
                Feels_Like = weatherData.FeelsLike,
                Temp_Min = weatherData.TempMin,
                Temp_Max = weatherData.TempMax,
                Pressure = weatherData.Pressure,
                Humidity = weatherData.Humidity,
                Wind_Speed = weatherData.WindSpeed,
                Cloudiness = weatherData.Cloudiness,
                Weather_Main = weatherData.WeatherMain ?? "Unknown",
                Weather_Description = weatherData.WeatherDescription ?? "Unknown",
                Timestamp = (int)new DateTimeOffset(weatherData.Timestamp).ToUnixTimeSeconds(),
                User = user
            };

            await SaveWeatherHistoryAsync(connection, weatherHistory);

            return weatherData;
        }

        private async Task<User> GetOrCreateUserAsync(SqlConnection connection, string nickname, long chat_id)
        {
            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE nickname = @nickname",
                new { nickname });

            if (user == null)
            {
                await connection.ExecuteAsync(
                    "INSERT INTO Users (nickname, isBanned, chat_id) VALUES (@nickname, 0, @chat_id)",
                    new { nickname, chat_id });

                user = await connection.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM Users WHERE nickname = @nickname",
                    new { nickname });
            }

            return user;
        }

        private async Task SaveWeatherHistoryAsync(SqlConnection connection, WeatherHistory history)
        {
            await connection.ExecuteAsync(
                @"INSERT INTO WeatherHistory(user_id, city, temperature, feels_like, temp_min, temp_max, pressure,
                humidity, wind_speed, cloudiness, weather_main, weather_description, timestamp)
                VALUES (@User_Id, @City, @Temperature, @Feels_Like, @Temp_Min, @Temp_Max, @Pressure,
                @Humidity, @Wind_Speed, @Cloudiness, @Weather_Main, @Weather_Description, @Timestamp)",
                history);
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}