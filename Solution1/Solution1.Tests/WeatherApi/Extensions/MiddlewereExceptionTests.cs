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
using System.Threading.Tasks;
using WeatherApi.Infrastructure;
using Xunit;

namespace Weather.Tests.WeatherApi.Extensions
{
    public class MiddlewereExceptionTests
    {
        private readonly Mock<ILogger<ExceptionMiddleware>> _logger;
        private readonly Mock<RequestDelegate> _delegate;
        private readonly ExceptionMiddleware _exceptionMiddleware;

        public static IEnumerable<object[]> ParamsForExceptionHandlingTest =>
            new List<object[]>
            {
                new object[] { StatusCodes.Status500InternalServerError, new Exception() },
                new object[] { StatusCodes.Status408RequestTimeout, new OperationCanceledException() },
                new object[] 
                { 
                    StatusCodes.Status400BadRequest, 
                    new ValidationException(
                        "Test message",
                        new List<ValidationFailure>()
                        {
                            new ValidationFailure(nameof(ForecastWeatherRequestDTO.CityName), "'City Name' must not be empty."),
                            new ValidationFailure(nameof(ForecastWeatherRequestDTO.PeriodOfDays), "'Period Of Days' must be between 0 and 5. You entered -1.")
                        }) 
                }
            };

        public MiddlewereExceptionTests()
        {
            _logger = new Mock<ILogger<ExceptionMiddleware>>();
            _delegate = new Mock<RequestDelegate>();
            _exceptionMiddleware = new ExceptionMiddleware(_delegate.Object, _logger.Object);
        }

        [Theory]
        [MemberData(nameof(ParamsForExceptionHandlingTest))]
        public async Task InvokeAsync_ExceptionHandling_Success(int statusCode, Exception exception)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _delegate.Setup(x => x.Invoke(It.IsAny<HttpContext>())).Throws(exception);
            string responseBody = default;

            //Act
            using (httpContext.Response.Body = new MemoryStream())
            {
                await _exceptionMiddleware.InvokeAsync(httpContext);
                httpContext.Response.Body.Position = 0;
                responseBody = new StreamReader(httpContext.Response.Body).ReadToEnd();                
            }            

            //Assert
            _delegate.Verify(x => x.Invoke(It.IsAny<HttpContext>()));
            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(x => x.GetType() == exception.GetType()),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            Assert.Equal(statusCode, httpContext.Response.StatusCode);

            if (exception.GetType() == typeof(ValidationException))
            {
                var actual = JsonSerializer.Deserialize<ValidationProblemDetails>(responseBody);
                var expected = new ValidationProblemDetails(
                      new Dictionary<string, string[]>()
                      {
                          {nameof(ForecastWeatherRequestDTO.CityName), new string[] { "'City Name' must not be empty." }},
                          {nameof(ForecastWeatherRequestDTO.PeriodOfDays), new string[] { "'Period Of Days' must be between 0 and 5. You entered -1." }}
                      });
                Assert.True(new CompareLogic().Compare(expected, actual).AreEqual);
            }
        }
    }
}
