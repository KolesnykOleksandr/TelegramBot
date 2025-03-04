using Microsoft.AspNetCore.Mvc;
using TelegramBot.Dtos;
using TelegramBot.Interfaces;
using TelegramBot.Models;

namespace TelegramBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        public readonly IWeatherRepository _weatherRepository;
        public WeatherController(IWeatherRepository weatherRepository)
        {
            _weatherRepository = weatherRepository;
        }


        [HttpGet("GetWeather/{city}")]
        public async Task<IActionResult> GetWeather(string city)
        {
            var result = await _weatherRepository.GetWeatherAsync(city, "admin", 853719718);
            return Ok(result);
        }
    }
}
