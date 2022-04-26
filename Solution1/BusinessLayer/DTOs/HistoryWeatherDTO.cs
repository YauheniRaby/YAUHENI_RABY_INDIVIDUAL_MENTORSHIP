using System.Collections.Generic;

namespace BusinessLayer.DTOs
{
    public class HistoryWeatherDTO
    {
        public string CityName { get; set; }

        public IEnumerable<WeatherWithDatetimeDTO> WeatherList { get; set; }
    }
}
