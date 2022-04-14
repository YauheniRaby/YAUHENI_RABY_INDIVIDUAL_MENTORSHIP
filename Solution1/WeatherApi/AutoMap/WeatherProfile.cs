using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Extensions;
using DataAccessLayer.Models;

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
            CreateMap<WeatherResponseDTO, Weather>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Datetime, opt => opt.Ignore())
                .ForMember(x => x.Comment, opt => opt.Ignore());
            CreateMap<WeatherResponseDTO, WeatherResponse>()
                .ForMember(dest => dest.Message, conf => conf.MapFrom(src => src.ErrorMessage))
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.DateTime, opt => opt.Ignore());

        }
    }
}