using System;

namespace BusinessLayer.DTOs
{
    public class WeatherWithDateTimeDTO : BaseWeatherDTO
    {
        public DateTime DateTime { get; set; }
    }
}
