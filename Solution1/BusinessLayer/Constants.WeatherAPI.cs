using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public static partial class Constants
    {
        public class WeatherAPI
        {
            public const string KeyApi = "3fe39edadae3ae57d133a80598d5b120";
            public const string UrlApi = "https://api.openweathermap.org/data/2.5/weather?q=cityName&appid=keyApi&units=metric";
        }        
    }
}
