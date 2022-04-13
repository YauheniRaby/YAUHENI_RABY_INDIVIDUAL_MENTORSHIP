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
        private readonly IOptionsMonitor<BackgroundJobConfiguration> _configMonitor;

        public BackgroundJobFilter(IBackgroundJobClient backgroundJobsClinent, IOptionsMonitor<BackgroundJobConfiguration> configMonitor)
        {
            _backgroundJobsClinent = backgroundJobsClinent;
            _configMonitor = configMonitor;
        }
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            _configMonitor.OnChange(appConfig => EnqueueBackgroundJob());
            EnqueueBackgroundJob();

            return next;
        }

        private void EnqueueBackgroundJob()
        {
            _backgroundJobsClinent.Enqueue<IBackgroundJobService>(x => x.UpdateJobs(_configMonitor.CurrentValue.CitiesOptions));
        }
    }
}
