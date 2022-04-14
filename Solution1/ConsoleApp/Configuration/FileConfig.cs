using System.Configuration;
using ConsoleApp.Configuration.Abstract;

namespace ConsoleApp.Configuration
{
    public class FileConfig : IConfig
    {
        public AppConfiguration AppConfig { get => new AppConfiguration(); }

        public WeatherAriConfiguration ApiConfig { get => new WeatherAriConfiguration(); }

        public DbConfiguration DbConfig { get => new DbConfiguration(); }
    }

    public class AppConfiguration
    {
        public int MinCountDaysForecast => int.TryParse(ConfigurationManager.AppSettings["minCountDays"], out var value) ? value : default;

        public int MaxCountDaysForecast => int.TryParse(ConfigurationManager.AppSettings["maxCountDays"], out var value) ? value : default;

        public bool IsDebugMode => bool.TryParse(ConfigurationManager.AppSettings["isDebugMode"], out var value) ? value : default;

        public int? RequestTimeout => int.TryParse(ConfigurationManager.AppSettings["requestTimeout"], out var value) ? value : null;
    }

    public class WeatherAriConfiguration
    {
        public int CountPointsInDay => int.TryParse(ConfigurationManager.AppSettings["countPointsInDay"], out var value) ? value : default;

        public string Key => ConfigurationManager.AppSettings["key"];

        public string CurrentWeatherUrl => ConfigurationManager.AppSettings["currentWeatherUrl"];

        public string ForecastWeatherUrl => ConfigurationManager.AppSettings["forecastWeatherUrl"];

        public string CoordinatesUrl => ConfigurationManager.AppSettings["coordinatesUrl"];
    }

    public class DbConfiguration
    {
        public string ConnectionString => ConfigurationManager.AppSettings["connectionString"];
    }
}
