using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Application.JsonResponses;

namespace TelegramBot.Application.Interfaces
{
    public interface IWeatherApiService
    {
        Task<WeatherApiResponse> GetWeatherAsync(string city);
    }
}
