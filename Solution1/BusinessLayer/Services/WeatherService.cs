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
        
        public async Task<ForecastWeatherDTO> GetForecastByCityNameAsync(DataForWeatherRequestDTO dataForWeatherRequestDTO)
        {
            var countPointForCurrentDay = 
                (DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow).Hours /
                (24/Constants.WeatherAPI.CountWeatherPointinDay); 
            var countWeatherPoint = dataForWeatherRequestDTO.PeriodOfDays * Constants.WeatherAPI.CountWeatherPointinDay + countPointForCurrentDay;

            var forecast = await _weatherApiService.GetForecastByCityNameAsync(dataForWeatherRequestDTO.CityName, countWeatherPoint);
            forecast.City.Name = dataForWeatherRequestDTO.CityName;

            return _mapper.Map<ForecastWeatherDTO>(forecast);
        }        
    }
}
