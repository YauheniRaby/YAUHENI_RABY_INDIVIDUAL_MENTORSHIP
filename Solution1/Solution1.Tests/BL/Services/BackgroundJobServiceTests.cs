using AutoMapper;
using BusinessLayer;
using BusinessLayer.DTOs;
using BusinessLayer.Services;
using BusinessLayer.Services.Abstract;
using DataAccessLayer.Repository.Abstract;
using Hangfire;
using Hangfire.Common;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherApi.AutoMap;
using Xunit;

namespace Weather.Tests.BL.Services
{
    public class BackgroundJobServiceTests
    {
        private readonly Mock<IRecurringJobManager> _recurringJobManagerMock;
        private readonly Mock<IWeatherServiсe> _weatherServiсeMock;
        private readonly IMapper _mapper;
        private readonly Mock<IWeatherRepository> _weatherRepositoryMock;
        private readonly Mock<JobStorage> _jobStorageMock;
        private readonly Mock<ILogger<BackgroundJobService>> _loggerMock;
        private readonly BackgroundJobService _backgroundJobService;
        private readonly string cityName = "Minsk";

        public BackgroundJobServiceTests()
        {
            _recurringJobManagerMock = new Mock<IRecurringJobManager>();
            _weatherServiсeMock = new Mock<IWeatherServiсe>();
            _mapper = _mapper = new Mapper(MapperConfig.GetConfiguration());
            _weatherRepositoryMock = new Mock<IWeatherRepository>();
            _jobStorageMock = new Mock<JobStorage>();
            _loggerMock = new Mock<ILogger<BackgroundJobService>>();
            _backgroundJobService = new BackgroundJobService(_weatherServiсeMock.Object, _weatherRepositoryMock.Object, _recurringJobManagerMock.Object, _jobStorageMock.Object, _mapper, _loggerMock.Object);
        }
        
        [Fact]
        public async Task GetWeather_EnterCityName_Success()
        {
            // Arrange
            var comment = "Test Comment";
            var temp = 10;
            var weatherDto = new WeatherDTO() { CityName = cityName, Comment = comment, Temp = temp };
            
            _weatherServiсeMock
                .Setup(weatherService =>
                    weatherService.GetByCityNameAsync
                    (It.Is<string>(x => x == cityName),
                     It.IsAny<CancellationToken>()))
                .ReturnsAsync(weatherDto);

            // Act
            await _backgroundJobService.GetWeather(cityName);

            // Assert
            _weatherServiсeMock.Verify(weatherService => weatherService.GetByCityNameAsync(It.Is<string>(x => x == cityName), It.IsAny<CancellationToken>()));
            _weatherRepositoryMock.Verify(
                repository =>
                    repository.AddWeatherAsync(
                        It.Is<DataAccessLayer.Models.Weather>(x => 
                            x.Comment == comment 
                            && x.Temp == temp 
                            && x.CityName == cityName 
                            && x.Datatime != default(DateTime))));
        }

        [Fact]
        public async Task GetWeather_HandlingException_Success()
        {
            // Act
            await _backgroundJobService.GetWeather(cityName);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));
        }

        [Fact]
        public async Task UpdateJobs_EnterArrayCitiesOptions_Success()
        {
            // Arrange
            var cityName2 = "Berlin";
            var cityName3 = "Paris";
            var timeout = 10;
            var timeout2 = 20;
            
            var arrayJobs = new HashSet<string>() { cityName.ToLower(), cityName3.ToLower() };
            var ciriesOptionsDto = new List<CityOptionDTO>() 
            { 
                new CityOptionDTO() { CityName = cityName, Timeout = timeout },
                new CityOptionDTO() { CityName = cityName2, Timeout = timeout2 }
            };
            var storageConnection = new Mock<IStorageConnection>();
            _jobStorageMock.Setup(jobStorage => jobStorage.GetConnection()).Returns(storageConnection.Object);
            
            storageConnection
                .Setup(storageConnection => 
                    storageConnection.GetAllItemsFromSet(
                        It.Is<string>(x => x == Constants.Hangfire.RecurringJobs)))
                .Returns(arrayJobs);
            
            // Act
            await _backgroundJobService.UpdateJobs(ciriesOptionsDto);

            // Assert
            storageConnection.Verify(x => x.GetAllItemsFromSet(It.Is<string>(x => x == Constants.Hangfire.RecurringJobs)));
            _recurringJobManagerMock.Verify(x => x.RemoveIfExists(It.Is<string>(x => x == cityName3.ToLower())), Times.Once);
            _recurringJobManagerMock.Verify(
                x => x.AddOrUpdate(
                    It.IsAny<string>(),
                    It.IsAny<Job>(),
                    It.IsAny<string>(),
                    It.IsAny<RecurringJobOptions>()), Times.Exactly(2));
            _recurringJobManagerMock.Verify(
                x => x.AddOrUpdate(
                    It.Is<string>(x => x == cityName.ToLower()),
                    It.Is<Job>(x => x.Method.Name == nameof(BackgroundJobService.GetWeather)),
                    It.Is<string>(x => x == Cron.MinuteInterval(timeout)),
                    It.Is<RecurringJobOptions>(x => x.TimeZone.Id == TimeZoneInfo.Utc.Id)));
            _recurringJobManagerMock.Verify(
                x => x.AddOrUpdate(
                    It.Is<string>(x => x == cityName2.ToLower()),
                    It.Is<Job>(x => x.Method.Name == nameof(BackgroundJobService.GetWeather)),
                    It.Is<string>(x => x == Cron.MinuteInterval(timeout2)),
                    It.Is<RecurringJobOptions>(x => x.TimeZone.Id == TimeZoneInfo.Utc.Id)));
        }
    }
}
