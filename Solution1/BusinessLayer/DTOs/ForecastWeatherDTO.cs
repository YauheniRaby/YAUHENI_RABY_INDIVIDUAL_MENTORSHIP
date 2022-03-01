using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class ForecastWeatherDTO
    {
        public List<WeatherForDate> WeatherValuesForPeriod { get; set; }

        public string CityName { get; set; }
    }

    public class WeatherForDate
    {
        public DateTime DateTime { get; set; }

        public double Temp { get; set; }
        
        public string Comment { get; set; }
    }


}
