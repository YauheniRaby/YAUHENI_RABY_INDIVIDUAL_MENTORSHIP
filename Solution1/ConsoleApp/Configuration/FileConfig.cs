using ConsoleApp.Configuration.Abstract;

namespace ConsoleApp.Configuration
{
    public class FileConfig : IConfig
    {
        public AppConfiguration AppConfiguration { get; set; }

        public WeatherApiConfiguration WeatherApiConfiguration { get; set; }
    }

    public class AppConfiguration
    {
        public int MinCountDaysForecast { get; set; }

        public int MaxCountDaysForecast { get; set; }

        public bool IsDebugMode { get; set; }

        public int? RequestTimeout { get; set; }
    }

    public class WeatherApiConfiguration
    {
        public int CountPointsInDay { get; set; }

        public string Key { get; set; }

        public string CurrentWeatherUrl { get; set; }

        public string ForecastWeatherUrl { get; set; }

        public string CoordinatesUrl { get; set; }
    }
}
