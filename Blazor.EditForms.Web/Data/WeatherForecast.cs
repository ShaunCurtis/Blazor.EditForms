/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System;

namespace Blazor.EditForms.Web.Data
{
    public class WeatherForecast
    {
        public Guid ID { get; set; }

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }

        public WeatherForecast Copy()
            => new()
            {
                ID = this.ID,
                Date = this.Date,
                TemperatureC = this.TemperatureC,
                Summary = this.Summary
            };
    }
}
