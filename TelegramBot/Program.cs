using Microsoft.Extensions.Configuration;
using TelegramBot;
using TelegramBot.Interfaces;
using TelegramBot.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


TelegramBotHost bot = new TelegramBotHost("8165256332:AAEaCZ8H3SBHY-XlLuehE8ndaUFl_XtQrYY", builder.Configuration);
bot.Start();


builder.Services.AddSingleton<TelegramBotHost>(bot);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
