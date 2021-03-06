using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.DTOs.Enums;
using BusinessLayer.Exceptions;
using BusinessLayer.Services;
using BusinessLayer.Services.Abstract;
using DataAccessLayer.Repositories.Abstract;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeatherApi.AutoMap;
using Xunit;

namespace Weather.Tests.BL.Services
{
    public class SaveWeatherServiceTests
    {
        private readonly SaveWeatherService _saveWeatherService;
        private readonly Mock<IHistoryWeatherService> _historyWeatherServiceMock;
        private readonly Mock<IWeatherServiсe> _weatherServiceMock;
        private readonly IMapper _mapper;
        private readonly string _cityName = "Minsk";
        private readonly int _temp = -5;
        private readonly string _cityName2 = "Paris";
        private readonly int _temp2 = 10;
        private readonly List<string> _cityNameList;
        private readonly string _currentWeatherUrl = "http://test.com/current/{0}/";


        public SaveWeatherServiceTests()
        {
            _cityNameList = new List<string>() { _cityName, _cityName2 };
            _weatherServiceMock = new Mock<IWeatherServiсe>();
            _historyWeatherServiceMock = new Mock<IHistoryWeatherService>();
            _mapper = new Mapper(MapperConfig.GetConfiguration());
            _saveWeatherService = new SaveWeatherService(_weatherServiceMock.Object, _historyWeatherServiceMock.Object, _mapper);
        }

        [Fact]
        public async Task AddByArrayCityNameAsync_EnterArrayCitiesName_Success()
        {
            // Arrange            
            var comment = DataAccessLayer.Constants.WeatherComments.DressWarmly; 
            var comment2 = DataAccessLayer.Constants.WeatherComments.Fresh;
            
            SetWeatherServiceSettings(ResponseStatus.Successful);            

            //Act
            await _saveWeatherService.AddByArrayCityNameAsync(_cityNameList, _currentWeatherUrl, CancellationToken.None);

            // Assert
            _weatherServiceMock.Verify(service =>
                service.GetWeatherByArrayCityNameAsync(
                        It.Is<IEnumerable<string>>(
                            x => x.Count() == 2
                            && x.Contains(_cityName)
                            && x.Contains(_cityName2)),
                        _currentWeatherUrl,
                        It.Is<CancellationToken>(
                            x => !x.IsCancellationRequested)));

            _historyWeatherServiceMock.Verify(service =>
                service.BulkSaveWeatherListAsync(
                    It.Is<List<DataAccessLayer.Models.Weather>>(
                        x => x.Count == 2
                        && x.Any(weather => weather.CityName == _cityName && weather.Temp == _temp && weather.Comment == comment && weather.Datetime.Day == DateTime.UtcNow.Day)
                        && x.Any(weather => weather.CityName == _cityName2 && weather.Temp == _temp2 && weather.Comment == comment2 && weather.Datetime.Day == DateTime.UtcNow.Day)
                        ),
                    It.Is<CancellationToken>(x => !x.IsCancellationRequested)));

            _historyWeatherServiceMock.VerifyNoOtherCalls();
        }


        [Fact]
        public async Task AddByArrayCityNameAsync_HandlingFailResponse_ThrowException()
        {
            SetWeatherServiceSettings(ResponseStatus.Fail);            
            
            await Assert.ThrowsAsync<FailWeatherResponseException>(async () => await _saveWeatherService.AddByArrayCityNameAsync(_cityNameList, _currentWeatherUrl, CancellationToken.None));
        }

        [Fact]
        public async Task AddByArrayCityNameAsync_GenerateOperationCanceledException_Success()
        {
            await Assert.ThrowsAsync<OperationCanceledException>(
                async () => await _saveWeatherService.AddByArrayCityNameAsync(
                    _cityNameList,
                    _currentWeatherUrl,
                    new CancellationToken(true)));
        }

        private void SetWeatherServiceSettings(ResponseStatus responseStatus)
        {
            var weatherResponseList = new Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>()
                    {
                        {
                            responseStatus,
                            new List<WeatherResponseDTO>()
                            {
                                new WeatherResponseDTO() { CityName = _cityName, Temp = _temp, ResponseStatus = responseStatus},
                                new WeatherResponseDTO() { CityName = _cityName2, Temp = _temp2, ResponseStatus = responseStatus}
                            }
                        }
                    };

            _weatherServiceMock
                .Setup(service =>
                    service.GetWeatherByArrayCityNameAsync(
                        It.Is<IEnumerable<string>>(
                            x => x.Count() == 2
                            && x.Contains(_cityName)
                            && x.Contains(_cityName2)),
                        _currentWeatherUrl,
                        It.Is<CancellationToken>(
                            x => !x.IsCancellationRequested)))
                .ReturnsAsync(weatherResponseList);
        }
    }
}
