using TelegramBot;
using TelegramBot.Interfaces;
using TelegramBot.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


TelegramBotHost bot = new TelegramBotHost("8165256332:AAEaCZ8H3SBHY-XlLuehE8ndaUFl_XtQrYY", builder.Configuration);
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
