using TelegramBot;
using TelegramBot.Application.Interfaces;
using TelegramBot.Infrastructure.Repositories;
using TelegramBot.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(configure =>
{
    configure.ClearProviders();
    configure.AddProvider(new ConsoleLoggerProvider());
    configure.SetMinimumLevel(LogLevel.Debug);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

TelegramBotHost bot = new TelegramBotHost(builder.Configuration);
bot.Start();

builder.Services.AddSingleton<TelegramBotHost>(bot);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
