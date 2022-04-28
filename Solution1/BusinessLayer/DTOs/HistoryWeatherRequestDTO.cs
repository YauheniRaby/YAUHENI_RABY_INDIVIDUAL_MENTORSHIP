using Microsoft.AspNetCore.Mvc;
using System;

namespace BusinessLayer.DTOs
{
    public class HistoryWeatherRequestDTO
    {
        [FromRoute(Name = "cityName")]
        public string CityName { get; set; }
        
        public DateTime StartPeriod { get; set; }
        
        public DateTime EndPeriod { get; set; }
    }
}
