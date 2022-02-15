using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class WeatherShortDTO
    {
        public WeatherInfo Main { get; set; }

        [JsonPropertyName("name")]
        public string CityName { get; set; }        
    }

    public class WeatherInfo
    {
        public double Temp { get; set; }
    }
}
