using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.EditForms.Web.Data
{
    public class WeatherForecastDataService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private List<WeatherForecast> WeatherForecasts; 

        public WeatherForecastDataService()
            =>  WeatherForecasts = GetForecasts();

        public ValueTask<List<WeatherForecast>> GetWeatherForcastsAsync()
            => ValueTask.FromResult<List<WeatherForecast>>(WeatherForecasts);

        public ValueTask<WeatherForecast> GetWeatherForcastAsync(Guid id)
            => ValueTask.FromResult<WeatherForecast>(WeatherForecasts.FirstOrDefault(item => item.ID == id));

        public ValueTask<bool> SaveWeatherForcastAsync(WeatherForecast record)
        {
            var rec = WeatherForecasts.FirstOrDefault(item => item.ID == record.ID);
            if (rec != default)
            {
                rec.Date = record.Date;
                rec.TemperatureC = record.TemperatureC;
                rec.Summary = record.Summary;
            }
            return ValueTask.FromResult<bool>(rec != default);
        }


        private List<WeatherForecast> GetForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                ID = Guid.NewGuid(),
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToList();
        }
    }
}
