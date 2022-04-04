using AutoMapper;
using BusinessLayer.Services.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using System.Threading.Tasks;
using System;
using FluentValidation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BusinessLayer.DTOs.Enums;

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

        public async Task<WeatherDTO> GetByCityNameAsync(string cityName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var validationResult = await _validator
                                            .ValidateAsync(
                                                new ForecastWeatherRequestDTO() { CityName = cityName },
                                                options => options.IncludeRuleSets("CityName"),
                                                cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
           
            var weather = await _weatherApiService.GetByCityNameAsync(cityName, cancellationToken);
            var result = _mapper.Map<WeatherDTO>(weather).FillCommentByTemp();
            return result;
        }
        
        public async Task<ForecastWeatherDTO> GetForecastByCityNameAsync(string cityName, int countDay, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var countPointForCurrentDay = 
                (DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow).Hours /
                (24/Constants.WeatherAPI.WeatherPointsInDay); 
            var countWeatherPoint = countDay * Constants.WeatherAPI.WeatherPointsInDay + countPointForCurrentDay;

            var validationResult = await _validator
                                            .ValidateAsync(
                                                new ForecastWeatherRequestDTO() { CityName = cityName, PeriodOfDays = countDay },
                                                options => options.IncludeAllRuleSets(),
                                                cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var forecast = await _weatherApiService.GetForecastByCityNameAsync(cityName, countWeatherPoint, cancellationToken);
            forecast.City.Name = cityName;

            return _mapper.Map<ForecastWeatherDTO>(forecast).FillCommentByTemp(); 
        }

        public async Task<Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>> GetWeatherByArrayCityNameAsync(IEnumerable<string> cityNames, CancellationToken cancellationToken)
        {
            var timer = new Stopwatch();
            timer.Start();

            var listTasksRequest = cityNames.Select(async cityName =>
            {
                var weatherResponseDTO = new WeatherResponseDTO() { CityName = cityName };
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var validationResult = await _validator
                                            .ValidateAsync(
                                                new ForecastWeatherRequestDTO() { CityName = cityName },
                                                options => options.IncludeRuleSets("CityName"),
                                                cancellationToken);

                    if (validationResult.IsValid)
                    {
                        var temp = (await _weatherApiService.GetByCityNameAsync(cityName, cancellationToken))?.TemperatureValues.Temp;
                        if (temp.HasValue)
                        {
                            weatherResponseDTO.Temp = temp.Value;
                            weatherResponseDTO.ResponseStatus = ResponseStatus.Successful;
                        }
                        else
                        {
                            weatherResponseDTO.ResponseStatus = ResponseStatus.Fail;
                            weatherResponseDTO.ErrorMessage = Constants.Errors.UnknownError;
                        }
                    }
                    else
                    {
                        weatherResponseDTO.ResponseStatus = ResponseStatus.Fail;
                        weatherResponseDTO.ErrorMessage = validationResult.Errors.FirstOrDefault().ErrorMessage;
                    }
                }
                catch (OperationCanceledException)
                {
                    weatherResponseDTO.ErrorMessage = Constants.Errors.TimeoutExpired;
                    weatherResponseDTO.ResponseStatus = ResponseStatus.Canceled;
                }
                catch (Exception ex)
                {
                    weatherResponseDTO.ErrorMessage = ex.Message;
                    weatherResponseDTO.ResponseStatus = ResponseStatus.Fail;
                }
                weatherResponseDTO.LeadTime = timer.ElapsedMilliseconds;
                return weatherResponseDTO;
            });

            timer.Stop();

            var weatherResponses = await Task.WhenAll(listTasksRequest);
            var result = weatherResponses
                            .GroupBy(w => w.ResponseStatus)
                            .ToDictionary(k => k.Key, v => v.Select(response => response));
            return result;
        }
    }
}
