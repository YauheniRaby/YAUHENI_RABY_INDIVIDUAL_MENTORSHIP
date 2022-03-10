using System;
using System.Collections.Generic;

namespace BusinessLayer.DTOs
{
    public class ForecastWeatherDTO
    {
        public List<WeatherForDateDTO> WeatherForPeriod { get; set; }

        public string CityName { get; set; }
    }

    public class WeatherForDateDTO : BaseWeatherDTO
    {
        public DateTime DateTime { get; set; }
    }
}
