using BusinessLayer.Models;
using System.Collections.Generic;

namespace WeatherApi.Configuration
{
    public class BackgroundJobConfiguration
    {
        public IEnumerable<CityOption> CitiesOptions { get; set; }
    }
    
    
}