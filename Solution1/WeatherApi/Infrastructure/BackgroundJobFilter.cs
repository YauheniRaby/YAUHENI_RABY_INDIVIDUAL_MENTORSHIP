using BusinessLayer.DTOs;
using BusinessLayer.Models;
using BusinessLayer.Services.Abstract;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;

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
            _configMonitor.OnChange(appConfig => _backgroundJobsClinent.Enqueue<IBackgroundJobService>(x => x.UpdateJobs(appConfig.CitiesOptions)));
            _backgroundJobsClinent.Enqueue<IBackgroundJobService>(x => x.UpdateJobs(_configMonitor.CurrentValue.CitiesOptions));
            return next;
        }
    }
}
