using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TelegramBot.Application.Interfaces;
using TelegramBot.Application.JsonResponses;

namespace TelegramBot.Application.Services
{
    public class WeatherApiService : IWeatherApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:RequestUri"]);
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", _configuration["ApiSettings:RapidApiKey"]);
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", _configuration["ApiSettings:RapidApiHost"]);
        }

        public async Task<WeatherApiResponse> GetWeatherAsync(string city)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<WeatherApiResponse>($"{city}/UK");

                if (response == null)
                {
                    throw new Exception("Weather API returned null response");
                }

                if (response.Cod != 200)
                {
                    throw new Exception($"Weather API error: {response.Cod}");
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to get weather data: {ex.Message}", ex);
            }
        }
    }
}