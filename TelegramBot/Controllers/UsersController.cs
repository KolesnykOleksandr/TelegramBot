using Microsoft.AspNetCore.Mvc;
using TelegramBot.Interfaces;

namespace TelegramBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly TelegramBotHost _bot;

        public UsersController(IUserRepository userRepository, TelegramBotHost bot)
        {
            _userRepository = userRepository;
            _bot = bot;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userRepository.GetUsers();
            return Ok(result);
        }

        [HttpGet("GetUser/{user_id}")]
        public async Task<IActionResult> GetUser(int user_id)
        {
            var result = await _userRepository.GetUser(user_id);
            return Ok(result);
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


        [HttpPost("BanUser/{user_id}")]
        public async Task<IActionResult> BanUser(int user_id)
        {
            await _userRepository.BanUser(user_id);

            return Ok();
        }

        [HttpPost("UnbanUser/{user_id}")]
        public async Task<IActionResult> UnbanUser(int user_id)
        {
            await _userRepository.UnbanUser(user_id);

            return Ok();
        }

    }
}
