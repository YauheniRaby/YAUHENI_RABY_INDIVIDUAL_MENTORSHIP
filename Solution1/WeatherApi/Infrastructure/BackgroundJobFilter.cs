using BusinessLayer.Services;
using BusinessLayer.Services.Abstract;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using WeatherApi.Configuration;

namespace WeatherApi.Infrastructure
{
    public class BackgroundJobFilter : IStartupFilter
    {
        private readonly IBackgroundJobClient _backgroundJobsClinent;
        private readonly IOptionsMonitor<BackgroundJobConfiguration> _backgroundJobConfiguration;
        private readonly WetherApiConfiguration _apiConfiguration;
        private readonly ILogger<BackgroundJob> logger;

        public BackgroundJobFilter(IBackgroundJobClient backgroundJobsClinent, IOptionsMonitor<BackgroundJobConfiguration> backgroundJobConfiguration, IOptionsMonitor<WetherApiConfiguration> apiConfiguration, ILogger<BackgroundJob> logger)
        {
            _backgroundJobsClinent = backgroundJobsClinent;
            _backgroundJobConfiguration = backgroundJobConfiguration;
            _apiConfiguration = apiConfiguration.CurrentValue;

        }
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            _backgroundJobConfiguration.OnChange(appConfig => EnqueueBackgroundJob());
            EnqueueBackgroundJob();
            
            return next;
        }

        private void EnqueueBackgroundJob()
        {
            _backgroundJobsClinent.Enqueue<IBackgroundJobService>(x => x.UpdateJobs(_backgroundJobConfiguration.CurrentValue.CitiesOptions, _apiConfiguration.CurrentWeatherUrl, _apiConfiguration.Key));
        }
    }
}
