using TelegramBot;
using TelegramBot.Application.Interfaces;
using TelegramBot.Infrastructure.Repositories;
using TelegramBot.Infrastructure.Logging;
using TelegramBot.Application.Services;

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
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IWeatherApiService, WeatherApiService>();
builder.Services.AddSingleton<IWeatherRepository, WeatherRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

var app = builder.Build();

TelegramBotHost.Initialize(app.Configuration, app.Services);
TelegramBotHost.Start();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
