using AutoMapper;
using BusinessLayer.Services;
using BusinessLayer.DTOs;
using DataAccessLayer.Repositories.Abstract;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeatherApi.AutoMap;
using Xunit;
using DataAccessLayer.Models;
using KellermanSoftware.CompareNetObjects;

namespace Weather.Tests.BL.Services
{
    public class HistoryWeatherServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IWeatherRepository> _weatherRepository;
        private readonly HistoryWeatherService _historyWeatherService;
        private readonly HistoryWeatherRequestDTO _historyWeatherRequestDto;
        private readonly string _cityName = "Minsk";
        private readonly DateTime _startDateTime = new DateTime(2022, 01, 01, 10, 50, 00);
        private readonly DateTime _endDateTime = new DateTime(2022, 03, 03, 12, 30, 45);
        private readonly string _comment1 = "Comment1";
        private readonly string _comment2 = "Comment3";
        private readonly string _comment3 = "Comment4";
        private readonly DateTime _dateTime1 = new DateTime(2022, 02, 02, 15, 30, 00);
        private readonly DateTime _dateTime2 = new DateTime(2022, 02, 03, 10, 00, 00);
        private readonly DateTime _dateTime3 = new DateTime(2022, 02, 05, 16, 25, 00);
        private readonly int _temp1 = 10;
        private readonly int _temp2 = 11;
        private readonly int _temp3 = 12;
        private readonly List<DataAccessLayer.Models.Weather> _weatherList;

        public HistoryWeatherServiceTests()
        {
            _weatherRepository = new Mock<IWeatherRepository>();
            _mapper = new Mapper(MapperConfig.GetConfiguration());
            _historyWeatherRequestDto = new HistoryWeatherRequestDTO()
            {
                CityName = _cityName,
                StartPeriod = _startDateTime,
                EndPeriod = _endDateTime
            };
            _weatherList = new List<DataAccessLayer.Models.Weather>()
            {
                new DataAccessLayer.Models.Weather(){ CityName = _cityName, Comment = _comment1, Datetime = _dateTime1, Id = 1, Temp = _temp1 },
                new DataAccessLayer.Models.Weather(){ CityName = _cityName, Comment = _comment2, Datetime = _dateTime2, Id = 2, Temp = _temp2 },
                new DataAccessLayer.Models.Weather(){ CityName = _cityName, Comment = _comment3, Datetime = _dateTime3, Id = 3, Temp = _temp3 },
            };
            _historyWeatherService = new HistoryWeatherService(_weatherRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetByCityNameAndPeriodAsync_ReturnedHistoryWeather_Success()
        {
            // Arrange
            _weatherRepository
                .Setup(repository =>
                    repository.GetWeatherListAsync
                    (It.Is<string>(x => x == _cityName),
                     It.Is<DateTime>(x => x == _startDateTime),
                     It.Is<DateTime>(x => x == _endDateTime),
                     It.Is<CancellationToken>(x => !x.IsCancellationRequested)))
                .ReturnsAsync(_weatherList);

            // Act
            var result = await _historyWeatherService.GetByCityNameAndPeriodAsync(_historyWeatherRequestDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var expected = new HistoryWeatherDTO()
            {
                CityName = _cityName,
                WeatherList = new List<WeatherWithDateTimeDTO>()
                {
                    new WeatherWithDateTimeDTO() { Comment = _comment1, DateTime = _dateTime1, Temp = _temp1 },
                    new WeatherWithDateTimeDTO() { Comment = _comment2, DateTime = _dateTime2, Temp = _temp2 },
                    new WeatherWithDateTimeDTO() { Comment = _comment3, DateTime = _dateTime3, Temp = _temp3 }
                }
            };
            Assert.True(new CompareLogic().Compare(expected, result).AreEqual);
        }

        [Fact]
        public async Task GetByCityNameAndPeriodAsync_GenerateOperationCanceledException_Success()
        {
            await Assert.ThrowsAsync<OperationCanceledException>(
                async () => await _historyWeatherService.GetByCityNameAndPeriodAsync(
                    _historyWeatherRequestDto,
                    new CancellationToken(true)));
        }

        [Fact]
        public async Task BulkSaveWeatherListAsync_SaveWeatherList_Success()
        {
            // Act
            await _historyWeatherService.BulkSaveWeatherListAsync(_weatherList, CancellationToken.None);

            // Assert
            _weatherRepository.Verify(
                repository => repository.BulkSaveWeatherListAsync
                    (It.Is<IEnumerable<DataAccessLayer.Models.Weather>>(
                        x => x.Count() == _weatherList.Count
                        && x.Any(w => w.CityName == _cityName && w.Comment == _comment1 && w.Datetime == _dateTime1 && w.Temp == _temp1)
                        && x.Any(w => w.CityName == _cityName && w.Comment == _comment2 && w.Datetime == _dateTime2 && w.Temp == _temp2)
                        && x.Any(w => w.CityName == _cityName && w.Comment == _comment3 && w.Datetime == _dateTime3 && w.Temp == _temp3)),
                     It.Is<CancellationToken>(x => !x.IsCancellationRequested)));
        }

        [Fact]
        public async Task BulkSaveWeatherListAsync_GenerateOperationCanceledException_Success()
        {
            await Assert.ThrowsAsync<OperationCanceledException>(
                async () => await _historyWeatherService.BulkSaveWeatherListAsync(
                    _weatherList,
                    new CancellationToken(true)));
        }
    }
}
