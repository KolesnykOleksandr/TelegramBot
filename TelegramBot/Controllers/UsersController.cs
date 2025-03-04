using Microsoft.AspNetCore.Mvc;
using TelegramBot.Interfaces;

namespace TelegramBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
