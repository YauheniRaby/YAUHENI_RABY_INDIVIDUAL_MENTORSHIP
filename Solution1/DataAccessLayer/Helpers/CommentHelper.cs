namespace DataAccessLayer.Helpers
{
    public static class CommentHelper
    {
        public static string GetComment(double temp)
        {
            return temp switch
            {
                _ when temp < 0 => Constants.WeatherComments.DressWarmly,
                _ when temp >= 0 && temp < 20 => Constants.WeatherComments.Fresh,
                _ when temp >= 20 && temp < 30 => Constants.WeatherComments.GoodWeather,
                _ => Constants.WeatherComments.GoToBeach,
            };
        }
    }
}
