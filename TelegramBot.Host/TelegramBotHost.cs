using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Dtos;
using TelegramBot.Application.Interfaces;

namespace TelegramBot
{
    public static class TelegramBotHost
    {
        private static TelegramBotClient _bot;
        private static IServiceProvider _serviceProvider;
        private static ReplyKeyboardMarkup _replyButtons;

        public static void Initialize(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            var botToken = configuration["TelegramBotSettings:Token"]
                ?? throw new InvalidOperationException("Telegram bot token is missing in configuration.");

            _bot = new TelegramBotClient(botToken);
            _serviceProvider = serviceProvider;
            _replyButtons = new ReplyKeyboardMarkup(
                new KeyboardButton("🌤 Погода в Харкові"))
            {
                ResizeKeyboard = true
            };
        }

        public static void Start()
        {
            _bot.DeleteWebhook();

            _bot.StartReceiving(GettingMessageHanlder, ErrorHandler);
        }

        public static async Task<string?> SendWeatherToAll(string city)
        {
            using var scope = _serviceProvider.CreateScope();
            var weatherRepository = scope.ServiceProvider.GetRequiredService<IWeatherRepository>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            object weather = await weatherRepository.GetWeatherAsync(city, "admin", 0);
            if (weather is string)
            {
                return (weather as string);
            }

            string answer = $@"Адмін захотів усім повідомити про погоду у місті {city}
🌍Місто: {((WeatherDto)weather).City}
🌡Температура: {((WeatherDto)weather).Temperature}°C
🌡Температура відчувається: {((WeatherDto)weather).FeelsLike}°C
🌡Мінімальна температура: {((WeatherDto)weather).TempMin}°C
🌡Максимальна температура: {((WeatherDto)weather).TempMax}°C
🌬️Тиск: {((WeatherDto)weather).Pressure}Pa
💧Вологість: {((WeatherDto)weather).Humidity}%
💨Швидкість вітру: {((WeatherDto)weather).WindSpeed}м/с
☁️Хмарність: {((WeatherDto)weather).Cloudiness}%
🌤Погода: {((WeatherDto)weather).WeatherMain}
📋Опис погоди: {((WeatherDto)weather).WeatherDescription}
⏳Час: {((WeatherDto)weather).Timestamp}";

            var allusers = await userRepository.GetUsers();

            foreach (var user in allusers)
            {
                await _bot.SendMessage(user.chat_id, answer, replyMarkup: _replyButtons);
            }

            return null;
        }

        private static async Task ErrorHandler(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            Console.WriteLine(exception.Message);
        }

        private static async Task GettingMessageHanlder(ITelegramBotClient client, Update update, CancellationToken token)
        {
            Console.WriteLine($"{update.Message.Chat.FirstName} {update.Message.Chat.LastName} пише " + update.Message.Text);

            if (update.Message.Text == "🌤 Погода в Харкові")
            {
                await SendCityWeather("Харків", update);
            }
            else if (update.Message.Text == "/weather")
            {
                await _bot.SendMessage(update.Message?.Chat.Id, "Будь ласка після команди /weather впишіть назву вашого міста", replyMarkup: _replyButtons);
            }
            else if (update.Message.Text.StartsWith("/weather"))
            {
                string city = update.Message.Text.Substring(9);

                await SendCityWeather(city, update);
            }
            else
            {
                await _bot.SendMessage(update.Message?.Chat.Id, "Слава Україні", replyMarkup: _replyButtons);
            }
        }

        private static async Task SendCityWeather(string city, Update update)
        {
            using var scope = _serviceProvider.CreateScope();
            var weatherRepository = scope.ServiceProvider.GetRequiredService<IWeatherRepository>();

            object weather = await weatherRepository.GetWeatherAsync(city, update.Message.Chat.Username, update.Message.Chat.Id);

            if (weather is string)
            {
                await _bot.SendMessage(update.Message?.Chat.Id, weather as string, replyMarkup: _replyButtons);
                return;
            }

            string answer = $@"🌍Місто: {((WeatherDto)weather).City}
🌡Температура: {((WeatherDto)weather).Temperature}°C
🌡Температура відчувається: {((WeatherDto)weather).FeelsLike}°C
🌡Мінімальна температура: {((WeatherDto)weather).TempMin}°C
🌡Максимальна температура: {((WeatherDto)weather).TempMax}°C
🌬️Тиск: {((WeatherDto)weather).Pressure}Pa
💧Вологість: {((WeatherDto)weather).Humidity}%
💨Швидкість вітру: {((WeatherDto)weather).WindSpeed}м/с
☁️Хмарність: {((WeatherDto)weather).Cloudiness}%
🌤Погода: {((WeatherDto)weather).WeatherMain}
📋Опис погоди: {((WeatherDto)weather).WeatherDescription}
⏳Час: {((WeatherDto)weather).Timestamp}";

            await _bot.SendMessage(update.Message?.Chat.Id, answer);
        }
    }
}