using Microsoft.AspNetCore.Builder;
using WeatherApi.Infrastructure;

namespace WeatherApi.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseHttpStatusExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }        
    }
}
