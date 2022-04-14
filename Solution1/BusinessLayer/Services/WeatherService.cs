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
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Abstract;
using DataAccessLayer.Extensions;

namespace BusinessLayer.Services
{
    public class WeatherService : IWeatherServiсe
    {
        private readonly IMapper _mapper;
        private readonly IWeatherApiService _weatherApiService;
        private readonly IWeatherRepository _weatherRepository;
        private readonly IValidator<ForecastWeatherRequestDTO> _validator;

        public WeatherService(IMapper mapper, IWeatherApiService weatherApiService, IWeatherRepository weatherRepository, IValidator<ForecastWeatherRequestDTO> validator) 
        { 
            _mapper = mapper;
            _weatherApiService = weatherApiService;
            _weatherRepository = weatherRepository;
            _validator = validator;
        }

        public async Task<WeatherDTO> GetByCityNameAsync(string cityName, string currentWeatherUrl, string apiKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var validationResult = await _validator
                                            .ValidateAsync(
                                                new ForecastWeatherRequestDTO() { CityName = cityName },
                                                options => options.IncludeRuleSets(Constants.Validators.OnlyCityName),
                                                cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
           
            var weather = await _weatherApiService.GetByCityNameAsync(cityName, currentWeatherUrl, apiKey, cancellationToken);
            var result = _mapper.Map<WeatherDTO>(weather).FillCommentByTemp();
            return result;
        }
        
        public async Task<ForecastWeatherDTO> GetForecastByCityNameAsync(string cityName, int countDay, string forecastWeatherUrl, string coordinatesUrl, string apiKey, int countWeatherPointInDay, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var countPointForCurrentDay = 
                (DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow).Hours /
                (24/countWeatherPointInDay); 
            var countWeatherPoint = countDay * countWeatherPointInDay + countPointForCurrentDay;

            var validationResult = await _validator
                                            .ValidateAsync(
                                                new ForecastWeatherRequestDTO() { CityName = cityName, PeriodOfDays = countDay },
                                                options => options.IncludeAllRuleSets(),
                                                cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var forecast = await _weatherApiService.GetForecastByCityNameAsync(cityName, countWeatherPoint, forecastWeatherUrl, coordinatesUrl, apiKey, cancellationToken);
            forecast.City.Name = cityName;

            return _mapper.Map<ForecastWeatherDTO>(forecast).FillCommentByTemp(); 
        }

        public async Task<Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>> GetWeatherByArrayCityNameAsync(IEnumerable<string> cityNames, string currentWeatherUrl, string apiKey, CancellationToken cancellationToken)
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
                                                options => options.IncludeRuleSets(Constants.Validators.OnlyCityName),
                                                cancellationToken);

                    if (validationResult.IsValid)
                    {
                        var temp = (await _weatherApiService.GetByCityNameAsync(cityName, currentWeatherUrl, apiKey, cancellationToken))?.TemperatureValues.Temp;
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

        public async Task SaveWeatherListAsync(IEnumerable<string> cities, string currentWeatherUrl, string apiKey)
        {
            var weatherList = await GetWeatherByArrayCityNameAsync(cities, currentWeatherUrl, apiKey, CancellationToken.None);
            var dateTime = DateTime.UtcNow;

            if (weatherList.ContainsKey(ResponseStatus.Successful))
            {
                
                var resultWeatherList = _mapper.Map<List<Weather>>(weatherList[ResponseStatus.Successful]);
                resultWeatherList.ForEach(weather =>
                {
                    weather.Datetime = dateTime;
                    weather.FillCommentByTemp();
                });
                await _weatherRepository.BulkSaveWeatherListAsync(resultWeatherList);
            }

            if(weatherList.ContainsKey(ResponseStatus.Fail))
            {
                var failResponses = _mapper.Map<List<WeatherResponse>>(weatherList[ResponseStatus.Fail]).ToList();
                failResponses.ForEach(respose => respose.DateTime = dateTime);
            }
        }
    }
}
