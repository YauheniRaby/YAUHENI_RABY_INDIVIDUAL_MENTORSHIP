namespace WeatherApi.Configuration
{
    public class WeatherApiConfiguration
    {
        public string Key { get; set; }

        public string CurrentWeatherUrl { get; set; }

        public string ForecastWeatherUrl { get; set; }

        public string CoordinatesUrl { get; set; }

        public int CountPointsInDay { get; set; } 
    }
}
