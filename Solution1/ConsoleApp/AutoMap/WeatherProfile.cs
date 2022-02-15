using BusinessLayer.DTOs;
using DataAccessLayer.Model;

namespace ConsoleApp.AutoMap
{
    public class WeatherProfile : AutoMapper.Profile
    {
        public WeatherProfile()
        {
            CreateMap<WeatherShortDTO, WeatherDTO>()
                .ForMember(dest => dest.Temp, conf => conf.MapFrom(src => src.Main.Temp))
                .ForMember(dest => dest.Comment, conf => conf.MapFrom(src => GetCommentByTemp(src.Main.Temp)));
        }

        private string GetCommentByTemp(double temp)
        {
            return temp switch
            {
                double n when n < 0 => "Dress warmly.",
                double n when n >= 0 && n < 20 => "It's fresh",
                double n when n >= 20 && n < 30 => "Good weather.",
                _ => "It's time to go to the beach.",
            };
        }
    }
}
