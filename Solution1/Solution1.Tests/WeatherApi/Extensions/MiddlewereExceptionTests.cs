using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
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
                new object[] { StatusCodes.Status408RequestTimeout, new OperationCanceledException() }
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
            
            //Act
            await _exceptionMiddleware.InvokeAsync(httpContext);

            //Assert
            _delegate.Verify(x => x.Invoke(It.IsAny<HttpContext>()));
            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(x => new CompareLogic().Compare(x, exception).AreEqual),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            Assert.Equal(statusCode, httpContext.Response.StatusCode);
        }
    }
}
