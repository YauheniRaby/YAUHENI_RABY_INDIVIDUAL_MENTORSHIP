using BusinessLayer.Services.Abstract;
using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Storage;
using BusinessLayer.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Abstract;

namespace BusinessLayer.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly IMapper _mapper;
        private readonly IWeatherRepository _weatherRepository;

        public BackgroundJobService(IRecurringJobManager recurringJobManager, IWeatherServiсe weatherServiсe, IMapper mapper, IWeatherRepository weatherRepository)
        {
            _recurringJobManager = recurringJobManager;
            _weatherRepository = weatherRepository;
            _weatherServiсe = weatherServiсe;
            _mapper = mapper;
        }

        public void UpdateJobs(IEnumerable<CityOptionDTO> request)
        {
            var recurringJobs = JobStorage.Current.GetConnection().GetRecurringJobs();

            var newArrayCities = request.Select(x => x.CityName.ToLower()).ToList();
            var currentArrayCities = recurringJobs.Select(x => x.Id).ToList();

            currentArrayCities.Except(newArrayCities).ToList().ForEach(x => _recurringJobManager.RemoveIfExists(x));
            request.ToList().ForEach(x => _recurringJobManager.AddOrUpdate(x.CityName.ToLower(), () => GetWeather(x.CityName), Cron.MinuteInterval(x.Timeout)));
        }

        public async Task GetWeather(string cityName)
        {
            var weatherDto = await _weatherServiсe.GetByCityNameAsync(cityName, CancellationToken.None);
            var weather = _mapper.Map<Weather>(weatherDto);
            weather.Datatime = DateTime.Now;
            await _weatherRepository.AddWeatherAsync(weather);
        }
    }
}
