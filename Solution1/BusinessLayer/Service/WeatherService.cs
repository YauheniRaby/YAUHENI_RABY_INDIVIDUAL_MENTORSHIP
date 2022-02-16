using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Extension;
using BusinessLayer.Service.Abstract;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class WeatherService : IWeatherServiсe
    {
        private readonly IMapper _mapper;
        private readonly IWeatherApiService _weatherApiService;
        
        public WeatherService(IMapper mapper, IWeatherApiService weatherApiService) 
        { 
            _mapper = mapper;
            _weatherApiService = weatherApiService;
        }

        public async Task<WeatherDTO> GetByCityNameAsync(string cityName)
        {
            var weather = await _weatherApiService.GetByCityNameAsync(cityName);
            return _mapper.Map<WeatherDTO>(weather).GetCommentByTemp();
        }
    }
}
