﻿namespace BusinessLayer.DTOs
{
    public class WeatherResponseDTO
    {
        public string CityName { get; set; }
        public double? Temp { get; set;}
        public bool IsSuccessfulRequest { get; set; }
        public long LeadTime { get; set; }
        public string ErrorMessage { get; set; }
    }
}