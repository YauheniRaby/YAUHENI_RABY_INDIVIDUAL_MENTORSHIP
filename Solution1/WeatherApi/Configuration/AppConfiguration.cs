namespace WeatherApi.Configuration
{
    public class AppConfiguration
    {
        public int MinCountDaysForecast { get; set; }

        public int MaxCountDaysForecast { get; set; }

        public bool IsDebugMode { get; set; }

        public int? RequestTimeout { get; set; }
    }
}
