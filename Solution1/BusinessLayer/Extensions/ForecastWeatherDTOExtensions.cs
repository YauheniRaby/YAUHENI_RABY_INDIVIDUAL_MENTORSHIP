using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Extensions
{
    public static class ForecastWeatherDTOExtensions
    {
        public static string GetMultiStringRepresentation(this ForecastWeatherDTO weatherDTO)
        {
            var result = $"{weatherDTO.CityName} weather forecast:";
            int i = 0;

            foreach (var e in weatherDTO.WeatherValuesForPeriod)
            {                
                result += $"\nDay {i++} ({e.DateTime.ToString("D")}): {e.Temp:f1} C. {e.Comment}";
            }

            return result;
        }

        public static ForecastWeatherDTO FillCommentByTemp(this ForecastWeatherDTO weatherDTO)
        {
            weatherDTO.WeatherValuesForPeriod.ForEach(x => x.Comment = Comment.FillByTemp(x.Temp));
            return weatherDTO;
        }
    }
}
