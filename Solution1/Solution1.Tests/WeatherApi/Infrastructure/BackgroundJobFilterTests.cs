using BusinessLayer.Configuration;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using WeatherApi.Infrastructure;
using Xunit;

namespace Weather.Tests.WeatherApi.Infrastructure
{
    public class BackgroundJobFilterTests
    {
        private readonly Mock<IBackgroundJobClient> _backgroundJobsClinent;
        private readonly Mock<IOptionsMonitor<BackgroundJobConfiguration>> _configMonitor;
        private readonly BackgroundJobFilter _backgroundJobFilter;
        
        public BackgroundJobFilterTests()
        {
            _backgroundJobsClinent = new Mock<IBackgroundJobClient>();
            _configMonitor = new Mock<IOptionsMonitor<BackgroundJobConfiguration>>();
            _backgroundJobFilter = new BackgroundJobFilter(_backgroundJobsClinent.Object, _configMonitor.Object);
        }

        [Fact]
        public void Configure_SetStartupSettingsForBackgroundJob_Success()
        {
            //Arrange
            var citiesOptions = new List<CityOption>
            {
                new CityOption(){ CityName = "Minsk", Timeout = 10 },
                new CityOption(){ CityName = "Paris", Timeout = 15 },
            };
            
            _configMonitor
                .Setup(config => config.CurrentValue)
                .Returns(new BackgroundJobConfiguration() { CitiesOptions = citiesOptions });
            
            //Act
            _backgroundJobFilter.Configure(appBilder => { });

            //Assert
            var expectedJob = Job.FromExpression<IBackgroundJobService>(x => x.UpdateJobs(citiesOptions));

            _configMonitor.Verify(
                configMonitor => configMonitor.OnChange(It.IsAny<Action<BackgroundJobConfiguration, string>>()));

            _backgroundJobsClinent.Verify(
                client => client.Create(
                    It.Is<Job>(x => new CompareLogic().Compare(expectedJob, x).AreEqual),
                    It.IsAny<EnqueuedState>()));
        }
    }
}
