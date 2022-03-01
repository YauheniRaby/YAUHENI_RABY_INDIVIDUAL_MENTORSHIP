using AutoMapper;
using BusinessLayer.Services.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

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
        
        public async Task<ForecastWeatherDTO> GetForecastByCityNameAsync(string cityName, int countDay)
        {
            var countPointForCurrentDay = (DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow).Hours / 3; ///////////////////////
            var countWeatherPoint = countDay * 8 + countPointForCurrentDay;

            var forecast = await _weatherApiService.GetForecastByCityNameAsync(cityName, countWeatherPoint);
            forecast.City.Name = cityName;

            return _mapper.Map<ForecastWeatherDTO>(forecast);
        }        
    }
}
