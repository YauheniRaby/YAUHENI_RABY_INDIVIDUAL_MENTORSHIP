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
using BusinessLayer.Enum;
using BusinessLayer.Configuration.Abstract;

namespace BusinessLayer.Services
{
    public class WeatherService : IWeatherServiсe
    {
        private readonly IMapper _mapper;
        private readonly IWeatherApiService _weatherApiService;
        private readonly IValidator<ForecastWeatherRequestDTO> _validator;
        private readonly IConfig _config;

        public WeatherService(IMapper mapper, IWeatherApiService weatherApiService, IValidator<ForecastWeatherRequestDTO> validator, IConfig config) 
        { 
            _mapper = mapper;
            _weatherApiService = weatherApiService;
            _validator = validator;
            _config = config;
        }

        public async Task<WeatherDTO> GetByCityNameAsync(string cityName)
        {
            var cancellationToken = GetCancellationToken();
            var validationResult = await _validator
                                            .ValidateAsync(
                                                new ForecastWeatherRequestDTO() { CityName = cityName },
                                                options => options.IncludeRuleSets("CityName"));
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
           
            var weather = await _weatherApiService.GetByCityNameAsync(cityName, cancellationToken);
            var result = _mapper.Map<WeatherDTO>(weather).FillCommentByTemp();
            return result;
        }
        
        public async Task<ForecastWeatherDTO> GetForecastByCityNameAsync(string cityName, int countDay)
        {
            var cancellationToken = GetCancellationToken();
            var countPointForCurrentDay = 
                (DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow).Hours /
                (24/Constants.WeatherAPI.WeatherPointsInDay); 
            var countWeatherPoint = countDay * Constants.WeatherAPI.WeatherPointsInDay + countPointForCurrentDay;

            var validationResult = await _validator
                                            .ValidateAsync(
                                                new ForecastWeatherRequestDTO() { CityName = cityName, PeriodOfDays = countDay },
                                                options => options.IncludeAllRuleSets());
            
            var forecast = await _weatherApiService.GetForecastByCityNameAsync(cityName, countWeatherPoint, cancellationToken);
            forecast.City.Name = cityName;

            return _mapper.Map<ForecastWeatherDTO>(forecast).FillCommentByTemp(); 
        }

        public async Task<Dictionary<RequestStatus, IEnumerable<WeatherResponseDTO>>> GetWeatherByArrayCityNameAsync(IEnumerable<string> cityNames)
        {
            var timer = new Stopwatch();
            timer.Start();

            var cancellationToken = GetCancellationToken();
            var listTasksRequest = cityNames.Select(async cityName =>
            {
                var weatherResponseDTO = new WeatherResponseDTO() { CityName = cityName };
                try
                {
                    var validationResult = await _validator
                                            .ValidateAsync(
                                                new ForecastWeatherRequestDTO() { CityName = cityName },
                                                options => options.IncludeRuleSets("CityName"));

                    if (validationResult.IsValid)
                    {
                        var temp = (await _weatherApiService.GetByCityNameAsync(cityName, cancellationToken))?.TemperatureValues.Temp;
                        if (temp.HasValue)
                        {
                            weatherResponseDTO.Temp = temp.Value;
                            weatherResponseDTO.RequestStatus = RequestStatus.Successful;
                        }
                        else
                        {
                            weatherResponseDTO.ErrorMessage = "Unknown error";
                        }
                    }
                    else
                    {
                        weatherResponseDTO.ErrorMessage = validationResult.Errors.FirstOrDefault().ErrorMessage;
                    }
                }
                catch (TaskCanceledException)
                {
                    weatherResponseDTO.ErrorMessage = "Timeout exceeded";
                    weatherResponseDTO.RequestStatus = RequestStatus.Canceled;
                }
                catch (Exception ex)
                {
                    weatherResponseDTO.ErrorMessage = ex.Message;
                }
                weatherResponseDTO.LeadTime = timer.ElapsedMilliseconds;
                return weatherResponseDTO;
            });

            var weatherResponses = await Task.WhenAll(listTasksRequest);
            var result = weatherResponses
                            .GroupBy(w => w.RequestStatus)
                            .ToDictionary(k => k.Key, v => v.Select(response => response));
            return result;
        }
        
        private CancellationToken GetCancellationToken()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            if (_config.RequestTimeout.HasValue)
            {
                cancellationTokenSource.CancelAfter(_config.RequestTimeout.Value);
            }
            return cancellationTokenSource.Token;
        }
    }
}
