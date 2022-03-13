using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Weather.Tests.Infrastructure.Extensions
{
    public static  class HttpMessageHandlerMockExtensions
    {
        public static void GetSettings(this Mock<HttpMessageHandler> httpMessageHandler, HttpResponseMessage response, string uri)
        {
            httpMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(
                      request =>
                      request.Method == HttpMethod.Get
                      && request.RequestUri.ToString() == uri),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);
        }
    }
}
