using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Dtos;
using TelegramBot.Interfaces;
using TelegramBot.Repositories;

namespace TelegramBot
{
    public class TelegramBotHost
    {
        TelegramBotClient bot;

        private readonly IWeatherRepository _weatherRepository;
        private readonly IUserRepository _userRepository;
        ReplyKeyboardMarkup replyButtons;


        public TelegramBotHost(IConfiguration configuration)
        {
            var botToken = configuration["TelegramBotSettings:Token"]
               ?? throw new InvalidOperationException("Telegram bot token is missing in configuration.");

            bot = new TelegramBotClient(botToken);

            _weatherRepository = new WeatherRepository(configuration);
            _userRepository = new UserRepository(configuration);
            replyButtons = new ReplyKeyboardMarkup(
                new KeyboardButton("🌤 Погода в Харкові"))
            {
                ResizeKeyboard = true
            };
        }

        public void Start()
        {
            bot.StartReceiving(GettingMessageHanlder, ErrorHandler);
        }

        public async Task<string?> SendWeatherToAll(string city)
        {
            object weather = _weatherRepository.GetWeatherAsync(city, "admin", 0).Result;
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

            var allusers = _userRepository.GetUsers();

            foreach (var user in allusers.Result)
            {
                await bot.SendMessage(user.chat_id, answer, replyMarkup: replyButtons);
            }

            return null;
        }

        private async Task ErrorHandler(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            Console.WriteLine(exception.Message);
        }
        private async Task GettingMessageHanlder(ITelegramBotClient client, Update update, CancellationToken token)
        {
            Console.WriteLine($"{update.Message.Chat.FirstName} {update.Message.Chat.LastName} пише " + update.Message.Text);

            if (update.Message.Text == "🌤 Погода в Харькові")
            {
                await SendCityWeather("Харків", update);
            }
            else if (update.Message.Text == "/weather")
            {
                await bot.SendMessage(update.Message?.Chat.Id, "Будь ласка після команди /weather впишіть назву вашого міста", replyMarkup: replyButtons);
            }
            else if (update.Message.Text.StartsWith("/weather"))
            {
                string city = update.Message.Text.Substring(9);

                await SendCityWeather(city, update);
            }
            else
            {
                await bot.SendMessage(update.Message?.Chat.Id, "Слава Україні", replyMarkup: replyButtons);
            }
        }

        private async Task SendCityWeather(string city, Update update)
        {
            object weather = _weatherRepository.GetWeatherAsync(city, update.Message.Chat.Username, update.Message.Chat.Id).Result;

            if (weather is string)
            {
                await bot.SendMessage(update.Message?.Chat.Id, weather as string, replyMarkup: replyButtons);
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

            await bot.SendMessage(update.Message?.Chat.Id, answer);


        }
    }
}