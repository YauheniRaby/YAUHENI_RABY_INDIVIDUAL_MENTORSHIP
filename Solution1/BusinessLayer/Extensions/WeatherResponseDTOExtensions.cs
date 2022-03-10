using BusinessLayer.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Extensions
{
    public static class WeatherResponseDTOExtensions
    {
        public static string GetRepresentationSuccessResponse(this IEnumerable<WeatherResponseDTO> weatherResponseDTO)
        {
            return weatherResponseDTO
                    .Aggregate($"Success case:",
                        (result, next) => $"{result} \nCity: '{next.CityName}', Temp: {next.Temp}, Timer: {next.LeadTime}ms.");
        }

        public static string GetRepresentationFailResponse(this IEnumerable<WeatherResponseDTO> weatherResponseDTO)
        {
            return weatherResponseDTO
                    .Aggregate($"Fail case:",
                        (result, next) => $"{result} \nCity: '{next.CityName}', Error: {next.ErrorMessage}, Timer: {next.LeadTime}ms.");
        }
    }
}
