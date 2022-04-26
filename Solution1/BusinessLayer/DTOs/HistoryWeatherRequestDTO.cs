using System;

namespace BusinessLayer.DTOs
{
    public class HistoryWeatherRequestDTO
    {
        public string CityName { get; set; }

        public DateTime StartPeriod { get; set; }

        public DateTime EndPeriod { get; set; }
    }
}
