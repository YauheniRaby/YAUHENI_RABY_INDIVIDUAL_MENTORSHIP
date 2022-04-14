using System;

namespace DataAccessLayer.Models
{
    public class WeatherResponse
    {
        public int Id { get; set; }

        public string CityName { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }
    }
}