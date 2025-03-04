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


        public TelegramBotHost(string token, IConfiguration configuration)
        {
            bot = new TelegramBotClient(token);
            _weatherRepository = new WeatherRepository(configuration);
            _userRepository = new UserRepository(configuration);
            replyButtons = new ReplyKeyboardMarkup(new[]
{
                new KeyboardButton("🌤 Погода в Харкові")
            })
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
🌍Місто: {(weather as WeatherDto).City}
🌡Температура: {(weather as WeatherDto).Temperature}°C
🌡Температура відчувається: {(weather as WeatherDto).FeelsLike}°C
🌡Мінімальна температура: {(weather as WeatherDto).TempMin}°C
🌡Максимальна температура: {(weather as WeatherDto).TempMax}°C
🌬️Тиск: {(weather as WeatherDto).Pressure}Pa
💧Вологість: {(weather as WeatherDto).Humidity}%
💨Швидкість вітру: {(weather as WeatherDto).WindSpeed}м/с
☁️Хмарність: {(weather as WeatherDto).Cloudiness}%
🌤Погода: {(weather as WeatherDto).WeatherMain}
📋Опис погоди: {(weather as WeatherDto).WeatherDescription}
⏳Час: {(weather as WeatherDto).Timestamp}";

            var allusers = _userRepository.GetUsers();

            foreach (var user in allusers.Result)
            {
                bot.SendTextMessageAsync(user.chat_id, answer, replyMarkup: replyButtons);
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
                await SendCityWeather("Харків", client, update);
            }
            else if (update.Message.Text == "/weather")
            {
                client.SendTextMessageAsync(update.Message?.Chat.Id, "Будь ласка після команди /weather впишіть назву вашого міста", replyMarkup: replyButtons);
            }
            else if (update.Message.Text.StartsWith("/weather"))
            {
                string city = update.Message.Text.Substring(9);

                SendCityWeather(city, client, update);
            }
            else
            {
                client.SendTextMessageAsync(update.Message?.Chat.Id, "Слава Україні", replyMarkup: replyButtons);
            }


        }

        private async Task SendCityWeather(string city, ITelegramBotClient client, Update update) {
            object weather = _weatherRepository.GetWeatherAsync(city, update.Message.Chat.Username, update.Message.Chat.Id).Result;

            if (weather is string)
            {
                client.SendTextMessageAsync(update.Message?.Chat.Id, weather as string, replyMarkup: replyButtons);
                return;
            }

            string answer = $@"🌍Місто: {(weather as WeatherDto).City}
🌡Температура: {(weather as WeatherDto).Temperature}°C
🌡Температура відчувається: {(weather as WeatherDto).FeelsLike}°C
🌡Мінімальна температура: {(weather as WeatherDto).TempMin}°C
🌡Максимальна температура: {(weather as WeatherDto).TempMax}°C
🌬️Тиск: {(weather as WeatherDto).Pressure}Pa
💧Вологість: {(weather as WeatherDto).Humidity}%
💨Швидкість вітру: {(weather as WeatherDto).WindSpeed}м/с
☁️Хмарність: {(weather as WeatherDto).Cloudiness}%
🌤Погода: {(weather as WeatherDto).WeatherMain}
📋Опис погоди: {(weather as WeatherDto).WeatherDescription}
⏳Час: {(weather as WeatherDto).Timestamp}";

            client.SendTextMessageAsync(update.Message?.Chat.Id, answer);


        }

    } }