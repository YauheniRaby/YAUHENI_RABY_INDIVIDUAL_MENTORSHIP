using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class WeatherDTO
    {
        public string CityName { get; set; }
        public double Temp { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            return $"In {CityName} {Temp} C. {Comment}";
        }
    }

     
}
