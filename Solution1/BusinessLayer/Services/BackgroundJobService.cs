using BusinessLayer.Services.Abstract;
using Hangfire;
using System.Collections.Generic;
using System.Linq;
using Hangfire.Storage;
using BusinessLayer.DTOs;
using System.Threading;

namespace BusinessLayer.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IHistoryWeatherService _logWeatherServiсe;
        private readonly JobStorage _jobStorage;

        public BackgroundJobService(IHistoryWeatherService logWeatherServiсe, IRecurringJobManager recurringJobManager, JobStorage jobStorage)
        {
            _recurringJobManager = recurringJobManager;
            _logWeatherServiсe = logWeatherServiсe;
            _jobStorage = jobStorage;
        }

        public void UpdateJobs(IEnumerable<CityOptionDTO> request, string currentWeatherUrl)
        {
            var currentJobs = _jobStorage
                .GetConnection()
                .GetRecurringJobs()
                .Select(x => new {Name = x.Id , Timeout = x.Cron})
                .ToList();

            if(request == null)
            {
                currentJobs.ForEach(x => _recurringJobManager.RemoveIfExists(x.Name));
                return;
            }
            var dictionaryNewJobs = request
                .GroupBy(opt => $"*/{opt.Timeout} * * * *")
                .ToDictionary(k => k.Key, v => v.Select(opt => opt.CityName))
                .ToList();
            
            var newJobs = dictionaryNewJobs
                .Select(x => new { Name = GetJobName(x.Value), Timeout = x.Key })
                .ToList();

            var removeJobs = currentJobs
                .Where(currentJob => !newJobs.Any(newJob => newJob.Name == currentJob.Name && newJob.Timeout == currentJob.Timeout))
                .ToList();

            removeJobs.ForEach(x => _recurringJobManager.RemoveIfExists(x.Name));
            
            dictionaryNewJobs.ForEach(x => _recurringJobManager.AddOrUpdate(GetJobName(x.Value), () => _logWeatherServiсe.AddByArrayCityNameAsync(x.Value, currentWeatherUrl, CancellationToken.None), x.Key));
        }

        private string GetJobName(IEnumerable<string> cities)
        {
            return string.Join(';', cities.OrderBy(c => c)).ToLower();
        }     
    }
}
