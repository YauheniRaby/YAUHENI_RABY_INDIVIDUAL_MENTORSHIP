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
using WeatherApi.Configuration;
using WeatherApi.Infrastructure;
using Xunit;

namespace Weather.Tests.WeatherApi.Infrastructure
{
    public class BackgroundJobFilterTests
    {
        private readonly Mock<IBackgroundJobClient> _backgroundJobsClinent;
        private readonly Mock<IOptionsMonitor<BackgroundJobConfiguration>> _jobConfig;
        private readonly Mock<IOptionsMonitor<WeatherApiConfiguration>> _apiConfig;
        private readonly BackgroundJobFilter _backgroundJobFilter;
        private readonly string _weatherUrl = "test.com";

        public BackgroundJobFilterTests()
        {
            _backgroundJobsClinent = new Mock<IBackgroundJobClient>();
            _jobConfig = new Mock<IOptionsMonitor<BackgroundJobConfiguration>>();
            _apiConfig = new Mock<IOptionsMonitor<WeatherApiConfiguration>>();
            _backgroundJobFilter = new BackgroundJobFilter(_backgroundJobsClinent.Object, _jobConfig.Object, _apiConfig.Object);
        }

        [Fact]
        public void Configure_SetStartupSettingsForBackgroundJob_Success()
        {
            //Arrange
            var citiesOptions = new List<CityOptionDTO>
            {
                new CityOptionDTO(){ CityName = "Minsk", Timeout = 10 },
                new CityOptionDTO(){ CityName = "Paris", Timeout = 15 },
            };

            _jobConfig
                .Setup(config => config.CurrentValue)
                .Returns(new BackgroundJobConfiguration() { CitiesOptions = citiesOptions });

            _apiConfig
                .Setup(config => config.CurrentValue)
                .Returns(new WeatherApiConfiguration { CurrentWeatherUrl = _weatherUrl });

            //Act
            _backgroundJobFilter.Configure(appBilder => { });

            //Assert
            var expectedJob = Job.FromExpression<IBackgroundJobService>(x => x.UpdateJobs(citiesOptions, _weatherUrl));

            _jobConfig.Verify(
                configMonitor => configMonitor.OnChange(It.IsAny<Action<BackgroundJobConfiguration, string>>()));

            _backgroundJobsClinent.Verify(
                client => client.Create(
                    It.Is<Job>(x => new CompareLogic().Compare(expectedJob, x).AreEqual),
                    It.IsAny<EnqueuedState>()));
        }
    }
}
