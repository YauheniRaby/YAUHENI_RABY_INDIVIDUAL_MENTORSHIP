using BusinessLayer.DTOs;
using System.Collections.Generic;

namespace WeatherApi.Configuration
{
    public class BackgroundJobConfiguration
    {
        public IEnumerable<CityOptionDTO> CitiesOptions { get; set; }
    }
    
    
}