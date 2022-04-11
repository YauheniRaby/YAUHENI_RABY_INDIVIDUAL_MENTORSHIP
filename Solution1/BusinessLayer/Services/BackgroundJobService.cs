using BusinessLayer.Services.Abstract;
using System;
using System.Threading.Tasks;
using Hangfire;
using BusinessLayer.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Abstract;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly IMapper _mapper;
        private readonly IWeatherRepository _weatherRepository;
        private readonly JobStorage _jobStorage;
        private readonly ILogger<BackgroundJobService> _logger;

        public BackgroundJobService(IWeatherServiсe weatherServiсe, IWeatherRepository weatherRepository, IRecurringJobManager recurringJobManager, JobStorage jobStorage, IMapper mapper, ILogger<BackgroundJobService> logger)
        {
            _recurringJobManager = recurringJobManager;
            _weatherRepository = weatherRepository;
            _weatherServiсe = weatherServiсe;
            _mapper = mapper;
            _jobStorage = jobStorage;
            _logger = logger;
        }

        public Task UpdateJobs(IEnumerable<CityOptionDTO> request)
        {
            var currentArrayCities = _jobStorage
                .GetConnection()
                .GetAllItemsFromSet(Constants.Hangfire.RecurringJobs)
                .ToList();
            //var currentArrayCities = _jobStorage.GetConnection().GetRecurringJobs().Select(x => x.Id).ToList();

            var newArrayCities = request.Select(x => x.CityName.ToLower()).ToList();                       

            currentArrayCities.Except(newArrayCities).ToList().ForEach(x => _recurringJobManager.RemoveIfExists(x));
            request.ToList().ForEach(x => _recurringJobManager.AddOrUpdate(x.CityName.ToLower(), () => GetWeather(x.CityName), Cron.MinuteInterval(x.Timeout)));
            return Task.CompletedTask;
        }

        public async Task GetWeather(string cityName)
        {
            try
            {
                var weatherDto = await _weatherServiсe.GetByCityNameAsync(cityName, CancellationToken.None);
                var weather = _mapper.Map<Weather>(weatherDto);
                weather.Datatime = DateTime.Now;
                await _weatherRepository.AddWeatherAsync(weather);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }           
        }      
    }
}
