using DataAccessLayer.Model;
using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp.AutoMap
{
    public class WeatherProfile : AutoMapper.Profile 
    {
        public WeatherProfile()
        {
            CreateMap<Weather, WeatherDTO>()
                .ForMember(dest => dest.CityName, conf => conf.MapFrom(src => src.name))
                .ForMember(dest => dest.Temp, conf => conf.MapFrom(src => src.main.temp))
                .ForMember(dest => dest.Comment, conf => conf.MapFrom(src => GetCommentByTemp(src.main.temp)));
        }

        string GetCommentByTemp(double temp)
        {
            switch (temp)
            {
                case double n when (n < 0):
                    return "Dress warmly.";                    
                case double n when (n >= 0 && n<20 ):
                    return "It's fresh";
                case double n when (n >= 20 && n < 30):
                    return "Good weather.";
                default:
                    return "It's time to go to the beach.";
            }
        }
    }
}
