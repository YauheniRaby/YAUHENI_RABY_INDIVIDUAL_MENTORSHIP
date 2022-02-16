﻿using System.Text.Json.Serialization;

namespace BusinessLayer.DTOs
{
    public class WeatherApiDTO
    {
        [JsonPropertyName("main")]
        public WeatherApiTempsDTO TemperaturaValues { get; set; }

        [JsonPropertyName("name")]
        public string CityName { get; set; }        
    }

    public class WeatherApiTempsDTO
    {
        public double Temp { get; set; }
    }
}
