using BusinessLayer.DTOs;

namespace BusinessLayer.Extensions
{
    public static class WeatherDtoExtensions
    {
        public static string GetStringRepresentation(this WeatherDTO weatherDTO)
        {
            return $"In {weatherDTO.CityName} {weatherDTO.Temp} C. {weatherDTO.Comment}";
        }

        public static WeatherDTO FillCommentByTemp(this WeatherDTO weatherDTO)
        {
            weatherDTO.Comment = weatherDTO.Temp switch
            {
                _ when weatherDTO.Temp < 0 => "Dress warmly.",
                _ when weatherDTO.Temp >= 0 && weatherDTO.Temp < 20 => "It's fresh.",
                _ when weatherDTO.Temp >= 20 && weatherDTO.Temp < 30 => "Good weather.",
                _ => "It's time to go to the beach.",
            };
            return weatherDTO;
        }        
    }
}
