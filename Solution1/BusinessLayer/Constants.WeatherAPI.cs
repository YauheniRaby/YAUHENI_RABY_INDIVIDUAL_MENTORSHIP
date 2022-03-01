namespace BusinessLayer
{
    public static partial class Constants
    {
        public static class WeatherAPI
        {
            public const string KeyApi = "3fe39edadae3ae57d133a80598d5b120";
            public const string UrlApiForCurrentlyWeatherByCityName = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric";
            public const string UrlApiForCoordinatesByCityName = "http://api.openweathermap.org/geo/1.0/direct?q={0}&appid={1}";
            public const string UrlApiForForecastWeatherByCoordinates = "https://api.openweathermap.org/data/2.5/forecast?lat={0}&lon={1}&cnt={2}&units=metric&appid={3}";
            public const int CountWeatherPointinDay = 8;
        }        


    }
}



