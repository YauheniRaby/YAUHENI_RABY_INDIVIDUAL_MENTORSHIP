using ConsoleApp.Extensions;
using System.Net;
using Xunit;

namespace Weather.Tests.ConsoleApp.Extensions
{
    
    public class HttpStatusCodeExtensionsTests
    {
        [Fact]
        public void GetStringRepresentation_GetStringRepresentationFromHttpStatusCode_Success()
        {
            // Arrange
            HttpStatusCode? httpStatusCode = HttpStatusCode.BadRequest;

            // Act
            var result = httpStatusCode.GetStringRepresentation();
            
            // Assert
            var expected = "400 BadRequest.";
            Assert.Equal(expected, result);
        }
    }
}
