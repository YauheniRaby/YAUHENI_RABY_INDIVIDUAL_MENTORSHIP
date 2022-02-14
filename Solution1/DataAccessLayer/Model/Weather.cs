using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class Weather
    {
        public WeatherInfo Main { get; set; }
        public string Name { get; set; }
        
    }

    public class WeatherInfo
    {
        public double Temp { get; set; }
    }
}
