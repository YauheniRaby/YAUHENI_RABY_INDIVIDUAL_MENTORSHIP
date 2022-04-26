using System;

namespace DataAccessLayer.Models
{
    public class HistoryWeatherRequest
    {
        public string CityName { get; set; }

        public DateTime StartPeriod { get; set; }

        public DateTime EndPeriod { get; set; }
    }
}
