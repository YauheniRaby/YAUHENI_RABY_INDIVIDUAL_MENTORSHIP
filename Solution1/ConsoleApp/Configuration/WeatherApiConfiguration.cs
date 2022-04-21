using ConsoleApp.Configuration.Abstract;

namespace ConsoleApp.Configuration
{

    public class WeatherApiConfiguration
    {
        public int CountPointsInDay { get; set; }

        public string Key { get; set; }

        public string CurrentWeatherUrl { get; set; }

        public string ForecastWeatherUrl { get; set; }

        public string CoordinatesUrl { get; set; }
    }
}
