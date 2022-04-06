using BusinessLayer.DTOs;
using FluentValidation;
using FluentValidation.Results;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WeatherApi.Infrastructure;
using Xunit;

namespace Weather.Tests.WeatherApi.Extensions
{
    public class MiddlewereExceptionTests
    {
        private readonly Mock<ILogger<ExceptionMiddleware>> _loggerMock;
        private readonly Mock<RequestDelegate> _delegateMock;
        private readonly ExceptionMiddleware _exceptionMiddleware;

        public static IEnumerable<object[]> ParamsForExceptionHandlingTest =>
            new List<object[]>
            {
                new object[] { StatusCodes.Status500InternalServerError, new Exception() },
                new object[] { StatusCodes.Status408RequestTimeout, new OperationCanceledException()}                
            };

        public MiddlewereExceptionTests()
        {
            _loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            _delegateMock = new Mock<RequestDelegate>();
            _exceptionMiddleware = new ExceptionMiddleware(_delegateMock.Object, _loggerMock.Object);
        }

        [Theory]
        [MemberData(nameof(ParamsForExceptionHandlingTest))]
        public async Task InvokeAsync_ExceptionHandling_Success(int statusCode, Exception exception)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _delegateMock.Setup(x => x.Invoke(It.IsAny<HttpContext>())).Throws(exception);

            //Act
            await _exceptionMiddleware.InvokeAsync(httpContext);                           

            //Assert
            _delegateMock.Verify(x => x.Invoke(It.IsAny<HttpContext>()));
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(x => x.GetType() == exception.GetType()),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            Assert.Equal(statusCode, httpContext.Response.StatusCode);            
        }

        [Fact]
        public async Task InvokeAsync_HandlingValidationException_Success()
        {
            // Arrange
            var validationMessageForCityName = "City Name must not be empty.";
            var validationMessageForPeriodOfDays = "Period Of Days must be between 0 and 5. You entered 7.";

            var exception = new ValidationException(
                new List<ValidationFailure>()
                {
                    new ValidationFailure(nameof(ForecastWeatherRequestDTO.CityName), validationMessageForCityName),
                    new ValidationFailure(nameof(ForecastWeatherRequestDTO.PeriodOfDays), validationMessageForPeriodOfDays)
                });
            int actualStatusCode = default;

            var _contextMock = new Mock<HttpContext>();
            var _responserMock = new Mock<HttpResponse>();
            var _bodyMock = new Mock<Stream>();

            _responserMock.Setup(x => x.Body).Returns(_bodyMock.Object);
            _responserMock.Setup(x => x.HttpContext).Returns(_contextMock.Object);
            _contextMock.Setup(x => x.Response).Returns(_responserMock.Object);
            _responserMock.SetupSet(x => x.StatusCode = It.IsAny<int>()).Callback<int>(x => actualStatusCode = x);
            _delegateMock.Setup(x => x.Invoke(It.IsAny<HttpContext>())).Throws(exception);


            //Act
            await _exceptionMiddleware.InvokeAsync(_contextMock.Object);

            //Assert
            var response = new ValidationProblemDetails(
                    new Dictionary<string, string[]>()
                    {
                        { nameof(ForecastWeatherRequestDTO.CityName), new string[] { validationMessageForCityName } },
                        { nameof(ForecastWeatherRequestDTO.PeriodOfDays), new string[] { validationMessageForPeriodOfDays } }
                    });
            var arrayByteLength = new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(response)).Length;

            _bodyMock.Verify(x => x.WriteAsync(
                It.Is<ReadOnlyMemory<Byte>>(x => x.Length == arrayByteLength),
                It.IsAny<CancellationToken>()));

            _delegateMock.Verify(x => x.Invoke(It.IsAny<HttpContext>()));
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(x => x.GetType() == exception.GetType()),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            Assert.Equal(StatusCodes.Status400BadRequest, actualStatusCode);
        }
    }
}
