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
                        option.TimestampFormat = "dd.MM.yyyy HH:mm:ss";
                    });
            });
            return loggerFactory.CreateLogger<T>();
        }
    }
}
