using System.Text.Json.Serialization;

namespace BusinessLayer.DTOs
{
    public class WeatherApiDTO
    {
        [JsonPropertyName("main")]
        public WeatherApiTempDTO TemperatureValues { get; set; }

        [JsonPropertyName("name")]
        public string CityName { get; set; }        
    }

    public class WeatherApiTempDTO
    {
        public double Temp { get; set; }
    }
}
