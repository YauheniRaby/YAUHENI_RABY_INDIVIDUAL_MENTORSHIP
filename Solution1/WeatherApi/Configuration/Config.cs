using BusinessLayer.Configuration.Abstract;

namespace WeatherApi.Configuration
{
    public class Config : IConfig
    {
        public int MinCountDaysForecast { get; set; }

        public int MaxCountDaysForecast { get; set; }

        public bool IsDebugMode { get; set; }

        public int? RequestTimeout { get; set; }

        public string FormatDateTime { get; set; }
    }
}
