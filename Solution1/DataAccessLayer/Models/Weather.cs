using System;
namespace DataAccessLayer.Models
{
    public class Weather
    {
        public int Id { get; set; }

        public string CityName { get; set; }

        public string Comment { get; set; }

        public double Temp { get; set; }

        public DateTime Datetime { get; set; }
    }
}
