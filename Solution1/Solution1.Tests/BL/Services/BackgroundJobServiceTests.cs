using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services;
using BusinessLayer.Services.Abstract;
using Hangfire;
using Hangfire.Common;
using Hangfire.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using WeatherApi.AutoMap;
using Xunit;

namespace Weather.Tests.BL.Services
{
    public class BackgroundJobServiceTests
    {
        private readonly Mock<IRecurringJobManager> _recurringJobManagerMock;
        private readonly Mock<IWeatherServiсe> _weatherServiсeMock;
        private readonly Mock<JobStorage> _jobStorageMock;
        private readonly BackgroundJobService _backgroundJobService;
        
        public BackgroundJobServiceTests()
        {
            _recurringJobManagerMock = new Mock<IRecurringJobManager>();
            _weatherServiсeMock = new Mock<IWeatherServiсe>();
            _jobStorageMock = new Mock<JobStorage>();
            var mapper = new Mapper(MapperConfig.GetConfiguration());
            _backgroundJobService = new BackgroundJobService(_weatherServiсeMock.Object, _recurringJobManagerMock.Object, _jobStorageMock.Object, mapper);
        }

        [Fact]
        public void UpdateJobs_EnterArrayCitiesOptions_Success()
        {
            // Arrange
            var currentWeatherUrl = "test.com";

            var cityName1 = "Minsk";
            var cityName2 = "Berlin";
            var cityName3 = "Kiev";
            var cityName4 = "Paris";
            var cityName5 = "Moscow";
            var cityName6 = "Brest";

            var timeoutGroup1 = 10;
            var timeoutGroup2 = 15;
            var timeoutGroup3 = 20;
            var timeoutGroup4 = 25;
            var timeoutGroup5 = 30;

            var newOptions = new List<CityOptionDTO>
            {
                new CityOptionDTO(){ CityName = cityName1, Timeout = timeoutGroup1 },
                new CityOptionDTO(){ CityName = cityName2, Timeout = timeoutGroup5 },
                new CityOptionDTO(){ CityName = cityName3, Timeout = timeoutGroup2 },
                new CityOptionDTO(){ CityName = cityName4, Timeout = timeoutGroup2 },
                new CityOptionDTO(){ CityName = cityName5, Timeout = timeoutGroup1 },
                new CityOptionDTO(){ CityName = cityName6, Timeout = timeoutGroup4 },
            };

            var currentJobs = new List<RecurringJobDto>
            {
                new RecurringJobDto(){ Id = $"{cityName2}; {cityName1}".ToLower(), Cron = Cron.MinuteInterval(timeoutGroup1)},
                new RecurringJobDto(){ Id = $"{cityName3}; {cityName4}".ToLower(), Cron = Cron.MinuteInterval(timeoutGroup3)},
                new RecurringJobDto(){ Id = cityName5.ToLower(), Cron = Cron.MinuteInterval(timeoutGroup5)},
                new RecurringJobDto(){ Id = cityName6.ToLower(), Cron = Cron.MinuteInterval(timeoutGroup4)}
            };

            var storageConnectionMock = new Mock<IStorageConnection>();

            _jobStorageMock
                .Setup(storage => storage.GetConnection())
                .Returns(storageConnectionMock.Object);

            storageConnectionMock
                .Setup(s => s.GetAllItemsFromSet("recurring-jobs"))
                .Returns(new HashSet<string>(currentJobs.Select(x => x.Id)));

            currentJobs.ForEach(x =>
                {
                    storageConnectionMock
                        .Setup(s => s.GetAllEntriesFromHash($"recurring-job:{x.Id}"))
                        .Returns(new Dictionary<string, string>() { { "Cron", x.Cron } });
                });

            //Act
            _backgroundJobService.UpdateJobs(newOptions, currentWeatherUrl);

            // Assert
            _recurringJobManagerMock.Verify(x => x.RemoveIfExists(It.Is<string>(x => x == $"{cityName2}; {cityName1}".ToLower())), Times.Once);
            _recurringJobManagerMock.Verify(x => x.RemoveIfExists(It.Is<string>(x => x == $"{cityName3}; {cityName4}".ToLower())), Times.Once);
            _recurringJobManagerMock.Verify(x => x.RemoveIfExists(It.Is<string>(x => x == cityName5.ToLower())), Times.Once);

            var paramNewJobs = new[] {
                new { Name = $"{cityName1};{cityName5}".ToLower(), Cron = Cron.MinuteInterval(timeoutGroup1), Args = $"[\"{cityName1}\",\"{cityName5}\"]" },
                new { Name = $"{cityName3};{cityName4}".ToLower(), Cron = Cron.MinuteInterval(timeoutGroup2), Args = $"[\"{cityName3}\",\"{cityName4}\"]" },
                new { Name = cityName2.ToLower(), Cron = Cron.MinuteInterval(timeoutGroup5), Args = $"[\"{cityName2}\"]" },
                new { Name = cityName6.ToLower(), Cron = Cron.MinuteInterval(timeoutGroup4), Args = $"[\"{cityName6}\"]" }
            }.ToList();

            paramNewJobs.ForEach(option =>
                _recurringJobManagerMock.Verify(x =>
                    x.AddOrUpdate(
                        It.Is<string>(x => x == option.Name),
                        It.Is<Job>(x =>
                            x.Method.Name == nameof(WeatherService.SaveWeatherListAsync)
                            && x.Arguments.Length == 2
                            && x.Arguments[0] == option.Args
                            && x.Arguments[1] == $"\"{currentWeatherUrl}\""),
                        It.Is<string>(x => x == option.Cron),
                        It.Is<RecurringJobOptions>(x => x.TimeZone.Id == TimeZoneInfo.Utc.Id))));

            _recurringJobManagerMock.VerifyNoOtherCalls();
        }
    }
}
