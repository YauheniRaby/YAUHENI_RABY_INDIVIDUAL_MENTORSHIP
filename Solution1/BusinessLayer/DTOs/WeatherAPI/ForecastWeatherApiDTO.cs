using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessLayer.DTOs.WeatherAPI
{
    public class ForecastWeatherApiDTO
    {
        [JsonPropertyName("list")]
        public List<WeatherInfoApiDTO> WeatherPoints { get; set; }
       
        [JsonPropertyName("city")]
        public CityApiDTO City { get; set; }
    }

    public class CityApiDTO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }        
    }

    public class WeatherInfoApiDTO
    {
        [JsonPropertyName("main")]
        public TempApiDTO Temp { get; set; }

        [JsonPropertyName("dt_txt")]
        public DateTime DateTime { get; set; }
    }

    public class TempApiDTO
    {
        [JsonPropertyName("temp")]
        public double Value { get; set; }
    }
}
