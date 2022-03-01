using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class ForecastWeatherApiDTO
    {
        [JsonPropertyName("list")]
        public List<WeatherInfo> WeatherPoints { get; set; }
       
        [JsonPropertyName("city")]
        public City City { get; set; }
    }

    public class City
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }        
    }

    public class WeatherInfo
    {
        [JsonPropertyName("main")]
        public Temp Temp { get; set; }

        [JsonPropertyName("dt_txt")]
        public string Date { get; set; }
    }

    public class Temp
    {
        [JsonPropertyName("temp")]
        public double Value { get; set; }
    }
}
