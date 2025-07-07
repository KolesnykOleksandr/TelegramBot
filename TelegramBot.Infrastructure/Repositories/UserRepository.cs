using Dapper;
using System.Data.SqlClient;
using TelegramBot.Application.Interfaces;
using TelegramBot.Application.Models;
using TelegramBot.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace TelegramBot.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<User>?> GetUsers()
        {
            SqlConnection connection = GetConnection();
            List<User>? users = null;
            try
            {
                try
                {
                    users = (await connection.QueryAsync<User>("select * from Users")).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message, ex);
                }
            }
            finally
            {
                connection.Close();
            }

            return users;
        }

        public async Task<User?> GetUser(int user_id)
        {
            SqlConnection connection = GetConnection();

            User? user = null;

            try
            {
                try
                {

                    user = await connection.QueryFirstOrDefaultAsync<User>($"select * from Users Where user_id = @user_id", new { user_id });

                    if (user != null)
                    {
                        user.weather_history = (await connection.QueryAsync<WeatherHistory>(
                            $"SELECT * FROM WeatherHistory WHERE user_id = @user_id", new { user_id }
                        )).ToList();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message, ex);
                }
            }
            finally
            {
                connection.Close();
            }
            return user;
        }

        public async Task BanUser(int user_id)
        {
            SqlConnection connection = GetConnection();
            try
            {
                try
                {
                    await connection.ExecuteAsync($"UPDATE users SET isBanned = 1 WHERE user_id = @user_id", new { user_id });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message, ex);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task UnbanUser(int user_id)
        {
            SqlConnection connection = GetConnection();
            try
            {
                try
                {
                    await connection.ExecuteAsync($"UPDATE users SET isBanned = 0 WHERE user_id = @user_id", new { user_id });
                    await connection.ExecuteAsync($"UPDATE users SET isBanned = 1 WHERE user_id = @user_id", new { user_id });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message, ex);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

    }
}