using System.Text.Json.Serialization;

namespace BusinessLayer.DTOs
{
    public class CityCoordinatesDTO
    {
        [JsonPropertyName("name")]
        public string CityName { get; set; }
        
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        [JsonPropertyName("lon")]
        public double Longitude { get; set; }
    }
}