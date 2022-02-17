using Microsoft.Extensions.Logging;

namespace ConsoleApp.Configuration
{
    public class LoggerConfiguration
    {
        public static ILogger GetConfiguration<T>()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole();
            });
            return loggerFactory.CreateLogger<T>();
        }
    }
}
