using AutoMapper;
using BusinessLayer.DTOs;

namespace ConsoleApp.AutoMap
{
    public class WeatherProfile : Profile
    {
        public WeatherProfile()
        {
            CreateMap<WeatherApiDTO, WeatherDTO>()
                .ForMember(dest => dest.Temp, conf => conf.MapFrom(src => src.TemperaturaValues.Temp))
                .ForMember(dest => dest.Comment, opt => opt.Ignore());
        }
    }
}
