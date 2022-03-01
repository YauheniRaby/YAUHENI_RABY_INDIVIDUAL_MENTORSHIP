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
            weatherDTO.Comment = Comment.FillByTemp(weatherDTO.Temp);                
               
            return weatherDTO;
        }        
    }
}
