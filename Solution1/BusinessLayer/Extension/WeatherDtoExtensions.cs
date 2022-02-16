using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Extension
{
    public static class WeatherDtoExtensions
    {
        public static string PrintToString (this WeatherDTO weatherDTO)
        {
            return $"In {weatherDTO.CityName} {weatherDTO.Temp} C. {weatherDTO.Comment}";
        }

        public static WeatherDTO GetCommentByTemp(this WeatherDTO weatherDTO)
        {
            weatherDTO.Comment = weatherDTO.Temp switch
            {
                _ when weatherDTO.Temp < 0 => "Dress warmly.",
                _ when weatherDTO.Temp >= 0 && weatherDTO.Temp < 20 => "It's fresh",
                _ when weatherDTO.Temp >= 20 && weatherDTO.Temp < 30 => "Good weather.",
                _ => "It's time to go to the beach.",
            };
            return weatherDTO;
        }        
    }
}
