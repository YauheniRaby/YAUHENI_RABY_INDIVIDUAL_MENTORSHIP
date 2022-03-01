using AutoMapper;
using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp.AutoMap
{
    public class WeatherProfile : Profile
    {
        public WeatherProfile()
        {
            CreateMap<WeatherApiDTO, WeatherDTO>()
                .ForMember(dest => dest.Temp, conf => conf.MapFrom(src => src.TemperatureValues.Temp))
                .ForMember(dest => dest.Comment, opt => opt.Ignore());
            CreateMap<WeatherInfo, WeatherForDate>()
                .ForMember(dest => dest.DateTime, conf => conf.MapFrom(src => src.Date))
                .ForMember(dest => dest.Temp, conf => conf.MapFrom(src => src.Temp.Value))
                .ForMember(dest => dest.Comment, opt => opt.Ignore());
            CreateMap<ForecastWeatherApiDTO, ForecastWeatherDTO>()
                .ForMember(dest => dest.CityName, conf => conf.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.WeatherValuesForPeriod, conf => conf.MapFrom(src => GetMeanValueWeather(src.WeatherPoints)));
        }

        private IEnumerable<WeatherForDate> GetMeanValueWeather(IEnumerable<WeatherInfo> weathersInfoList)
        {
            return weathersInfoList
                    .GroupBy(w => Convert.ToDateTime(w.Date).Date)
                    .ToDictionary(x => x.Key, v => v.Select(w => w.Temp.Value).Sum() / v.Count())
                    .Select(x => new WeatherForDate() { DateTime = x.Key, Temp = x.Value });
        }
    }
}
