using AutoMapper;
using BusinessLayer.DTOs.Enums;
using BusinessLayer.Exceptions;
using BusinessLayer.Services.Abstract;
using DataAccessLayer.Extensions;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class SaveWeatherService : ISaveWeatherService
    {
        private readonly IWeatherServiсe _weatherService;
        private readonly IMapper _mapper;
        private readonly IWeatherRepository _weatherRepository;

        public SaveWeatherService(IWeatherServiсe weatherService, IWeatherRepository weatherRepository, IMapper mapper) 
        {
            _weatherService = weatherService;
            _weatherRepository = weatherRepository;
            _mapper = mapper;            
        }

        public async Task AddByArrayCityNameAsync(IEnumerable<string> cities, string currentWeatherUrl, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var weatherList = await _weatherService.GetWeatherByArrayCityNameAsync(cities, currentWeatherUrl, token);
            var dateTime = DateTime.UtcNow;
            var timeResult = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 0);

            if (weatherList.ContainsKey(ResponseStatus.Successful))
            {

                var resultWeatherList = _mapper.Map<List<Weather>>(weatherList[ResponseStatus.Successful]);
                resultWeatherList.ForEach(weather =>
                {
                    weather.Datetime = timeResult;
                    weather.FillCommentByTemp();
                });
                await _weatherRepository.BulkSaveWeatherListAsync(resultWeatherList, token);
            }

            if (weatherList.ContainsKey(ResponseStatus.Fail))
            {
                throw new BackgroundJobException(weatherList[ResponseStatus.Fail]);
            }
        }        
    }
}
