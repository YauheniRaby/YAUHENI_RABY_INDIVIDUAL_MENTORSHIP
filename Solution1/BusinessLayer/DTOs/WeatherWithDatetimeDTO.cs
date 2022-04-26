using System;

namespace BusinessLayer.DTOs
{
    public class WeatherWithDatetimeDTO : BaseWeatherDTO
    {
        public DateTime DateTime { get; set; }
    }
}
