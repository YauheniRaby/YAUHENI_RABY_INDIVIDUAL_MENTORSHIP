using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using WeatherApi.Infrastructure;
using Xunit;

namespace Weather.Tests.WeatherApi
{
    public class ExceptionMiddlewereTests
    {
        private readonly Mock<ILogger> _logger;
        private readonly Mock<RequestDelegate> _delegate;
        private readonly ExceptionMiddleware _exceptionMiddleware;

        public ExceptionMiddlewereTests()
        {
            _logger = new Mock<ILogger>();
            _delegate = new Mock<RequestDelegate>();
            _exceptionMiddleware = new ExceptionMiddleware(_delegate.Object, _logger.Object);
        }

        [Fact]
        public async Task InvokeAsync_ExceptionHandling_Success()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _delegate.Setup(x => x.Invoke(It.IsAny<HttpContext>())).Throws<Exception>();
            
            //Act
            await _exceptionMiddleware.InvokeAsync(httpContext);

            //Assert
            _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
        }
    }
}
