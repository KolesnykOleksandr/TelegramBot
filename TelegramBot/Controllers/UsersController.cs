using Microsoft.AspNetCore.Mvc;
using TelegramBot.Interfaces;

namespace TelegramBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("Starting to retrieve all users");

            try
            {
                var result = await _userRepository.GetUsers();
                _logger.LogInformation("Successfully retrieved {UserCount} users", result.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user list");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetUser/{user_id}")]
        public async Task<IActionResult> GetUser(int user_id)
        {
            _logger.LogDebug("Starting to retrieve user with ID: {UserId}", user_id);

            try
            {
                var result = await _userRepository.GetUser(user_id);

                if (result == null)
                {
                    _logger.LogWarning("User with ID {UserId} was not found", user_id);
                    return NotFound();
                }

                _logger.LogInformation("Successfully retrieved user {UserId}", user_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving user {UserId}", user_id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("BanUser/{user_id}")]
        public async Task<IActionResult> BanUser(int user_id)
        {
            _logger.LogInformation("Initiating ban for user {UserId}", user_id);

            try
            {
                await _userRepository.BanUser(user_id);
                _logger.LogInformation("Successfully banned user {UserId}", user_id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ban user {UserId}", user_id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("UnbanUser/{user_id}")]
        public async Task<IActionResult> UnbanUser(int user_id)
        {
            _logger.LogInformation("Initiating unban for user {UserId}", user_id);

            try
            {
                await _userRepository.UnbanUser(user_id);
                _logger.LogInformation("Successfully unbanned user {UserId}", user_id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unban user {UserId}", user_id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}