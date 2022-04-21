using ConsoleApp.Configuration.Abstract;

namespace ConsoleApp.Configuration
{
    public class FileConfig : IConfig
    {
        public AppConfiguration AppConfig { get; set; }

        public WeatherApiConfiguration ApiConfig { get; set; }

        public Test Test { get; set; }
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
    public class Test
    {
        public string Key { get; set; }
    }
}
