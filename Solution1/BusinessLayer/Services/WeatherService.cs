﻿using AutoMapper;
using BusinessLayer.Services.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using System.Threading.Tasks;
using System;
using FluentValidation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public async Task<WeatherDTO> GetByCityNameAsync(string cityName)
        {
            var validationResult = await _validator
                                            .ValidateAsync(
                                                new ForecastWeatherRequestDTO() { CityName = cityName}, 
                                                options => options.IncludeRuleSets("CityName"));
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var weather = await _weatherApiService.GetByCityNameAsync(cityName);
            var result = _mapper.Map<WeatherDTO>(weather).FillCommentByTemp();
            return result;
        }
        
        public async Task<ForecastWeatherDTO> GetForecastByCityNameAsync(string cityName, int countDay)
        {
            var countPointForCurrentDay = 
                (DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow).Hours /
                (24/Constants.WeatherAPI.WeatherPointsInDay); 
            var countWeatherPoint = countDay * Constants.WeatherAPI.WeatherPointsInDay + countPointForCurrentDay;

            var validationResult = await _validator
                                            .ValidateAsync(
                                                new ForecastWeatherRequestDTO() { CityName = cityName, PeriodOfDays = countDay },
                                                options => options.IncludeAllRuleSets());
            
            var forecast = await _weatherApiService.GetForecastByCityNameAsync(cityName, countWeatherPoint);
            forecast.City.Name = cityName;

            return _mapper.Map<ForecastWeatherDTO>(forecast).FillCommentByTemp(); 
        }

        public async Task<Dictionary<bool, List<WeatherResponseDTO>>> GetWeatherByArrayCityNameAsync(string arrayCityNames)
        {
            var cityNames = arrayCityNames.Split(',').Select(cityName => cityName.Trim());
            
            var timer = new Stopwatch();
            timer.Start();

            var listTasksRequest = cityNames.Select(async cityName =>
            {
                var weatherResponseDTO = new WeatherResponseDTO() { CityName = cityName };
                try
                {
                    ValidationCityName(cityName);
                    weatherResponseDTO.Temp = (await _weatherApiService.GetByCityNameAsync(cityName)).TemperatureValues.Temp;
                    weatherResponseDTO.IsSuccessfulRequest = true;
                }
                catch (ValidationException ex)
                {
                    weatherResponseDTO.IsSuccessfulRequest = false;
                    weatherResponseDTO.ErrorMessage = ex.Errors.FirstOrDefault().ErrorMessage;
                }
                catch (Exception ex)
                {
                    weatherResponseDTO.IsSuccessfulRequest = false;
                    weatherResponseDTO.ErrorMessage = ex.Message;
                }
                weatherResponseDTO.LeadTime = timer.ElapsedMilliseconds;
                return weatherResponseDTO;
            });

            var weatherResponses = await Task.WhenAll(listTasksRequest);
            return weatherResponses.GroupBy(w => w.IsSuccessfulRequest).ToDictionary(k => k.Key, v => v.ToList());
        }

        private void ValidationCityName(string cityName)
        {
            var validationResult = _validator
                                            .Validate(
                                                new ForecastWeatherRequestDTO() { CityName = cityName },
                                                options => options.IncludeRuleSets("CityName"));
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}
