using AutoMapper;
using BusinessLayer.Services.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using System.Threading.Tasks;
using System;
using FluentValidation;

namespace BusinessLayer.Services
{
    public class WeatherService : IWeatherServiсe
    {
        private readonly IMapper _mapper;
        private readonly IWeatherApiService _weatherApiService;
        private readonly IValidator<ForecastWeatherRequestDTO> _validator;


        public WeatherService(IMapper mapper, IWeatherApiService weatherApiService, IValidator<ForecastWeatherRequestDTO> validator) 
        { 
            _mapper = mapper;
            _weatherApiService = weatherApiService;
            _validator = validator;
        }

        public async Task<WeatherDTO> GetByCityNameAsync(ForecastWeatherRequestDTO forecastWeatherRequestDTO)
        {
            var validationResult = await _validator.ValidateAsync(forecastWeatherRequestDTO, options => options.IncludeRuleSets("CityName"));
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var weather = await _weatherApiService.GetByCityNameAsync(forecastWeatherRequestDTO.CityName);
            var result = _mapper.Map<WeatherDTO>(weather).FillCommentByTemp();
            return result;
        }
        
        public async Task<ForecastWeatherDTO> GetForecastByCityNameAsync(ForecastWeatherRequestDTO forecastWeatherRequestDTO)
        {
            var countPointForCurrentDay = 
                (DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow).Hours /
                (24/Constants.WeatherAPI.WeatherPointsInDay); 
            var countWeatherPoint = forecastWeatherRequestDTO.PeriodOfDays * Constants.WeatherAPI.WeatherPointsInDay + countPointForCurrentDay;

            var validationResult = await _validator.ValidateAsync(forecastWeatherRequestDTO, options => options.IncludeAllRuleSets());
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var forecast = await _weatherApiService.GetForecastByCityNameAsync(forecastWeatherRequestDTO.CityName, countWeatherPoint);
            forecast.City.Name = forecastWeatherRequestDTO.CityName;

            return _mapper.Map<ForecastWeatherDTO>(forecast);
        }        
    }
}
