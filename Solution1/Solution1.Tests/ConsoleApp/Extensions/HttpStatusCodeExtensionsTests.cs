using ConsoleApp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.ConsoleApp.Extensions
{
    
    public class HttpStatusCodeExtensionsTests
    {
        [Fact]
        public void GetStringRepresentation_GetStringRepresentationFromHttpStatusCode_Success()
        {
            // Arrange
            
            // Act
            var result = new HttpStatusCode?(HttpStatusCode.BadRequest).GetStringRepresentation();

            // Assert
            var expected = "400 BadRequest.";
            Assert.Equal(expected, result);
        }
    }
}
