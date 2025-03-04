using Dapper;
using System.Data.SqlClient;
using TelegramBot.Interfaces;
using TelegramBot.Models;

namespace TelegramBot.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<User>> GetUsers()
        {
            SqlConnection connection = GetConnection();
            List<User> users = (await connection.QueryAsync<User>("select * from Users")).ToList();


            return users;
        }
        public async Task<User> GetUser(int user_id)
        {
            SqlConnection connection = GetConnection();
            User? user = await connection.QueryFirstOrDefaultAsync<User>($"select * from Users Where user_id = {user_id}");

            if (user != null)
            {
                user.weather_history = (await connection.QueryAsync<WeatherHistory>(
                    $"SELECT * FROM WeatherHistory WHERE user_id = {user_id}"
                )).ToList();
            }

            return user;
        }

        public async Task BanUser(int user_id)
        {
            SqlConnection connection = GetConnection();
            await connection.ExecuteAsync($"UPDATE users SET isBanned = 1 WHERE user_id = {user_id}");
        }

        public async Task UnbanUser(int user_id)
        {
            SqlConnection connection = GetConnection();
            await connection.ExecuteAsync($"UPDATE users SET isBanned = 0 WHERE user_id = {user_id}");
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

    }
}
