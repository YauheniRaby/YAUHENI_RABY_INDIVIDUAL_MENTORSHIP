using System.Configuration;
using ConsoleApp.Configuration.Abstract;

namespace ConsoleApp.Configuration
{
    public class FileConfig : IConfig
    {
        public AppConfiguration AppConfig { get => new AppConfiguration(); }

        public WeatherApiConfiguration ApiConfig { get => new WeatherApiConfiguration(); }

        public DbConfiguration DbConfig { get => new DbConfiguration(); }
    }

    public class AppConfiguration
    {
        public int MinCountDaysForecast { get; set; }

        public int MaxCountDaysForecast { get; set; }

        public bool IsDebugMode { get; set; }

        public int? RequestTimeout { get; set; }

        public AppConfiguration()
        {
            MinCountDaysForecast = int.TryParse(ConfigurationManager.AppSettings["minCountDays"], out var minCount) ? minCount : default;
            MaxCountDaysForecast = int.TryParse(ConfigurationManager.AppSettings["maxCountDays"], out var maxCount) ? maxCount : default;
            IsDebugMode = bool.TryParse(ConfigurationManager.AppSettings["isDebugMode"], out var isdebug) ? isdebug : default;
            RequestTimeout = int.TryParse(ConfigurationManager.AppSettings["requestTimeout"], out var timeout) ? timeout : null;
        }
    }

    public class WeatherApiConfiguration
    {
        public int CountPointsInDay { get; set; }

        public string Key { get; set; }

        public string CurrentWeatherUrl { get; set; }

        public string ForecastWeatherUrl { get; set; }

        public string CoordinatesUrl { get; set; }

        public WeatherApiConfiguration()
        {
            CountPointsInDay = int.TryParse(ConfigurationManager.AppSettings["countPointsInDay"], out var value) ? value : default;
            Key = ConfigurationManager.AppSettings["key"];
            CurrentWeatherUrl = ConfigurationManager.AppSettings["currentWeatherUrl"];
            ForecastWeatherUrl = ConfigurationManager.AppSettings["forecastWeatherUrl"];
            CoordinatesUrl = ConfigurationManager.AppSettings["coordinatesUrl"];
        }
    }

    public class DbConfiguration
    {
        public string ConnectionString { get; set; }

        public DbConfiguration()
        {
            ConnectionString = ConfigurationManager.AppSettings["connectionString"];
        }
    }
}
