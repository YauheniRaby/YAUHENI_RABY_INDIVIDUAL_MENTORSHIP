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
                    .AddConsole(option =>
                    {
                        option.TimestampFormat = BusinessLayer.Constants.Patterns.DateTime;
                    });
            });
            return loggerFactory.CreateLogger<T>();
        }
    }
}
