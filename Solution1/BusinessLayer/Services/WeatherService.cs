using AutoMapper;
using BusinessLayer.Services.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using System.Threading.Tasks;

namespace BusinessLayer.Services
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
            return _mapper.Map<WeatherDTO>(weather).FillCommentByTemp();
        }
    }
}
