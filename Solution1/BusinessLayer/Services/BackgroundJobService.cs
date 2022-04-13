using BusinessLayer.Services.Abstract;
using Hangfire;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Hangfire.Storage;
using BusinessLayer.Models;

namespace BusinessLayer.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly JobStorage _jobStorage;

        public BackgroundJobService(IWeatherServiсe weatherServiсe, IRecurringJobManager recurringJobManager, JobStorage jobStorage, IMapper mapper)
        {
            _recurringJobManager = recurringJobManager;
            _weatherServiсe = weatherServiсe;
            _jobStorage = jobStorage;
        }

        public void UpdateJobs(IEnumerable<CityOption> request)
        {
            var currentJobs = _jobStorage
                .GetConnection()
                .GetRecurringJobs()
                .Select(x => new {Name = x.Id , Timeout = x.Cron})
                .ToList();

            var dictionaryNewJobs = request
                .GroupBy(opt => Cron.MinuteInterval(opt.Timeout))
                .ToDictionary(k => k.Key, v => v.Select(opt => opt.CityName))
                .ToList();
            
            var newJobs = dictionaryNewJobs
                .Select(x => new { Name = GetJobName(x.Value), Timeout = x.Key })
                .ToList();

            var removeJobs = currentJobs
                .Where(currentJob => !newJobs.Any(newJob => newJob.Name == currentJob.Name && newJob.Timeout == currentJob.Timeout))
                .ToList();

            removeJobs.ForEach(x => _recurringJobManager.RemoveIfExists(x.Name));

            dictionaryNewJobs.ForEach(x => _recurringJobManager.AddOrUpdate(GetJobName(x.Value), () => _weatherServiсe.BackgroundSaveWeatherAsync(x.Value), x.Key));
        }

        private string GetJobName(IEnumerable<string> cities)
        {
            return cities
                    .OrderBy(c => c)
                    .Aggregate((result, next) => $"{result}; {next}")
                    .ToLower();
        }     
    }
}
