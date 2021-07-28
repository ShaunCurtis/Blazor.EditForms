using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.EditForms.Web.Data
{
    public class WeatherForecastViewService
    {
        private List<WeatherForecast> _records;
        private WeatherForecast _record;

        public List<WeatherForecast> Records
        {
            get => this._records;
            private set
            {
                this._records = value;
                ListChanged?.Invoke(this, EventArgs.Empty);

            }
        }

        public WeatherForecast Record
        {
            get => _record;
            set
            {
                this._record = value;
                RecordChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private WeatherForecastDataService DataService;

        public event EventHandler RecordChanged;

        public event EventHandler ListChanged;

        public WeatherForecastViewService(WeatherForecastDataService dataService)
        {
            this.DataService = dataService;
        }

        public async ValueTask GetWeatherForcastsAsync()
        {
            this.Records = await DataService.GetWeatherForcastsAsync();
        }

        public async ValueTask<bool> UpdateRecordAsync()
        {
            return await DataService.SaveWeatherForcastAsync(this.Record);
        }

        public async ValueTask<bool> GetRecordAsync(Guid id)
        {
            this.Record = await DataService.GetWeatherForcastAsync(id);
            return this.Record is not null;
        }
    }
}
