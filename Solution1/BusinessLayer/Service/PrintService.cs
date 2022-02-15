using BusinessLayer.DTOs;
using BusinessLayer.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class PrintService : IPrintService
    {
        public string Print(WeatherDTO weatherDTO)
        {
            return $"In {weatherDTO.CityName} {weatherDTO.Temp} C. {weatherDTO.Comment}";
        }
    }
}
