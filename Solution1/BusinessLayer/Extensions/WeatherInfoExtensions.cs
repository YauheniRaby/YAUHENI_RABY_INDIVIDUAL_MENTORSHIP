using BusinessLayer.DTOs;
using BusinessLayer.DTOs.WeatherAPI;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Extensions
{
    public static class WeatherInfoExtensions
    {
        public static IEnumerable<WeatherForDateDTO> GetMeanValueWeather(this IEnumerable<WeatherInfoApiDTO> weathersInfoList)
        {
            return weathersInfoList
                        .GroupBy(w => w.DateTime.Date)
                        .ToDictionary(x => x.Key, v => v.Select(w => w.Temp.Value).Sum() / v.Count())
                        .Select(x => new WeatherForDateDTO() { DateTime = x.Key, Temp = x.Value });
        }
    }
}
