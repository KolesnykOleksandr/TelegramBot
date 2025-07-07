using System;
using System.Collections.Generic;
using TelegramBot.Application.Dtos;

namespace TelegramBot.Application.JsonResponses
{
    public class WeatherApiResponse
    {
        public Coord Coord { get; set; }
        public List<Weather> Weather { get; set; }
        public string Base { get; set; }
        public Main Main { get; set; }
        public int Visibility { get; set; }
        public Wind Wind { get; set; }
        public Clouds Clouds { get; set; }
        public long Dt { get; set; }
        public Sys Sys { get; set; }
        public int Timezone { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cod { get; set; }

        public WeatherDto ToWeatherDto()
        {
            return new WeatherDto
            {
                City = this.Name ?? "Unknown",
                Temperature = ConvertFahrenheitToCelsius(this.Main.Temp),
                FeelsLike = ConvertFahrenheitToCelsius(this.Main.Feels_Like),
                TempMin = ConvertFahrenheitToCelsius(this.Main.Temp_Min),
                TempMax = ConvertFahrenheitToCelsius(this.Main.Temp_Max),
                Pressure = this.Main.Pressure,
                Humidity = this.Main.Humidity,
                WindSpeed = this.Wind.Speed,
                Cloudiness = this.Clouds.All,
                WeatherMain = this.Weather.FirstOrDefault()?.Main ?? "Unknown",
                WeatherDescription = this.Weather.FirstOrDefault()?.Description ?? "Unknown",
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(this.Dt).UtcDateTime
            };
        }

        private float ConvertFahrenheitToCelsius(float fahrenheit)
        {
            return (float)Math.Round((fahrenheit - 32) / 1.8f, 1);
        }
    }

    public class Coord
    {
        public double Lon { get; set; }
        public double Lat { get; set; }
    }

    public class Weather
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    public class Main
    {
        public float Temp { get; set; }
        public float Feels_Like { get; set; }
        public float Temp_Min { get; set; }
        public float Temp_Max { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public int Sea_Level { get; set; }
        public int Grnd_Level { get; set; }
    }

    public class Wind
    {
        public float Speed { get; set; }
        public int Deg { get; set; }
        public float Gust { get; set; }
    }

    public class Clouds
    {
        public int All { get; set; }
    }

    public class Sys
    {
        public string Country { get; set; }
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
    }
}