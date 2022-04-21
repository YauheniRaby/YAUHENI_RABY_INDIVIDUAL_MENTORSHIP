using BusinessLayer.Services.Abstract;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using WeatherApi.Configuration;

namespace WeatherApi.Infrastructure
{
    public class BackgroundJobFilter : IStartupFilter
    {
        private readonly IBackgroundJobClient _backgroundJobsClinent;
        private readonly IOptionsMonitor<BackgroundJobConfiguration> _backgroundJobConfiguration;
        private readonly IOptionsMonitor<WeatherApiConfiguration> _apiConfiguration;

        public BackgroundJobFilter(IBackgroundJobClient backgroundJobsClinent, IOptionsMonitor<BackgroundJobConfiguration> backgroundJobConfiguration, IOptionsMonitor<WeatherApiConfiguration> apiConfiguration)
        {
            _backgroundJobsClinent = backgroundJobsClinent;
            _backgroundJobConfiguration = backgroundJobConfiguration;
            _apiConfiguration = apiConfiguration;

        }
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            _backgroundJobConfiguration.OnChange(appConfig => EnqueueBackgroundJob());
            EnqueueBackgroundJob();
            
            return next;
        }

        private void EnqueueBackgroundJob()
        {
            _backgroundJobsClinent.Enqueue<IBackgroundJobService>(
                x => x.UpdateJobs(
                    _backgroundJobConfiguration.CurrentValue.CitiesOptions, 
                    $"{_apiConfiguration.CurrentValue.CurrentWeatherUrl}{_apiConfiguration.CurrentValue.Key}"));
        }
    }
}
