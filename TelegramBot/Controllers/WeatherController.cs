﻿using Microsoft.AspNetCore.Mvc;
using TelegramBot.Interfaces;

namespace TelegramBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        public readonly IWeatherRepository _weatherRepository;
        private readonly TelegramBotHost _bot;

        public WeatherController(IWeatherRepository weatherRepository, TelegramBotHost bot)
        {
            _weatherRepository = weatherRepository;
            _bot = bot;
        }


        [HttpPost("WendWeatherToAll/{city}")]
        public async Task<IActionResult> PostWeather(string city)
        {
            var result = await _bot.SendWeatherToAll(city);

            if (result == null)
            {
                return Ok();
            }

            return StatusCode(404, result);
        }

        [HttpGet("GetWeather/{city}")]
        public async Task<IActionResult> GetWeather(string city)
        {
            var result = await _weatherRepository.GetWeatherAsync(city, "admin", 853719718);
            return Ok(result);
        }
    }
}
