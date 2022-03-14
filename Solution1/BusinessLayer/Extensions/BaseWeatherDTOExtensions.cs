using BusinessLayer.DTOs;

namespace BusinessLayer.Extensions
{
    public static class BaseWeatherDTOExtensions
    {
        public static T FillCommentByTemp <T> (this T weatherDTO)
            where T : BaseWeatherDTO
        {
            weatherDTO.Comment = weatherDTO.Temp switch
            {
                _ when weatherDTO.Temp < 0 => Constants.WeatherComments.DressWarmly,
                _ when weatherDTO.Temp >= 0 && weatherDTO.Temp < 20 => Constants.WeatherComments.Fresh,
                _ when weatherDTO.Temp >= 20 && weatherDTO.Temp < 30 => Constants.WeatherComments.GoodWeather,
                _ => Constants.WeatherComments.GoToBeach,
            };

            return weatherDTO;
        }
    }
}
