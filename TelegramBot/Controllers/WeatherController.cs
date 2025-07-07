using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TelegramBot.Application.Interfaces;

namespace TelegramBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(
            IWeatherRepository weatherRepository,
            IConfiguration configuration,
            ILogger<WeatherController> logger)
        {
            _weatherRepository = weatherRepository;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("SendWeatherToAll/{city}")]
        public async Task<IActionResult> PostWeather(string city)
        {
            _logger.LogInformation("Starting to send weather for {City} to all users", city);

            try
            {
                var result = await TelegramBotHost.SendWeatherToAll(city);

                if (result == null)
                {
                    _logger.LogInformation("Successfully sent weather for {City} to all users", city);
                    return Ok();
                }

                _logger.LogWarning("Failed to send weather for {City} to all users. Error: {Error}", city, result);
                return StatusCode(404, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending weather for {City} to all users", city);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetWeather/{city}")]
        public async Task<IActionResult> GetWeather(string city)
        {
            _logger.LogInformation("Starting to retrieve weather for {City}", city);

            try
            {
                var adminId = _configuration.GetValue<long>("ChatIds:AdminTelegramId");
                _logger.LogDebug("Using admin Telegram ID: {AdminId}", adminId);

                var result = await _weatherRepository.GetWeatherAsync(city, "admin", adminId);

                _logger.LogInformation("Successfully retrieved weather for {City}", city);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving weather for {City}", city);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}