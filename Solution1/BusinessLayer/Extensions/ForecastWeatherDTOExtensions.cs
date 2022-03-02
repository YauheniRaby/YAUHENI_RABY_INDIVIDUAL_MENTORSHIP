using BusinessLayer.DTOs;
using System;
using System.Linq;

namespace BusinessLayer.Extensions
{
    public static class ForecastWeatherDTOExtensions
    {
        public static string GetMultiStringRepresentation(this ForecastWeatherDTO weatherDTO)
        {
            int i = 0;
            return weatherDTO.WeatherForPeriod
                    .Aggregate($"{weatherDTO.CityName} weather forecast:", 
                        (first, next) => $"{first} \nDay {i++} ({next.DateTime:D}): {next.Temp:f1} C. {next.Comment}");    
        }

        public static ForecastWeatherDTO FillCommentByTemp(this ForecastWeatherDTO weatherDTO)
        {
            weatherDTO.WeatherForPeriod.ForEach(x => x.FillCommentByTemp());
            return weatherDTO;
        }
    }
}
