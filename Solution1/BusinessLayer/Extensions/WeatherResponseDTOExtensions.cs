using BusinessLayer.DTOs;

namespace BusinessLayer.Extensions
{
    public static class WeatherResponseDTOExtensions
    {
        public static string GetRepresentationSuccessResponse(this WeatherResponseDTO response)
        {
            return $"City: '{response.CityName}', Temp: {response.Temp}, Timer: {response.LeadTime} ms.";
        }

        public static string GetRepresentationFailResponse(this WeatherResponseDTO response)
        {
            return $"City: '{response.CityName}', Error: {response.ErrorMessage}, Timer: {response.LeadTime} ms.";
        }
    }
}
