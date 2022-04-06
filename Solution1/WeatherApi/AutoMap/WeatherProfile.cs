using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Extensions;

namespace WeatherApi.AutoMap
{
    public class WeatherProfile : Profile
    {
        public WeatherProfile()
        {
            CreateMap<WeatherApiDTO, WeatherDTO>()
                .ForMember(dest => dest.Temp, conf => conf.MapFrom(src => src.TemperatureValues.Temp))
                .ForMember(dest => dest.Comment, opt => opt.Ignore());
            CreateMap<WeatherInfoApiDTO, WeatherForDateDTO>()
                .ForMember(dest => dest.Temp, conf => conf.MapFrom(src => src.Temp.Value))
                .ForMember(dest => dest.Comment, opt => opt.Ignore());
            CreateMap<ForecastWeatherApiDTO, ForecastWeatherDTO>()
                .ForMember(dest => dest.CityName, conf => conf.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.WeatherForPeriod, conf => conf.MapFrom(src => src.WeatherPoints.GetMeanValueWeather()));
        }
    }
}