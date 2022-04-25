using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace ConsoleApp.Configuration
{
    public class LoggerConfiguration
    {
        public static ILogger GetConfiguration<T>()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options => options.TimestampFormat = "dd.MM.yyyy HH:mm:ss");
            });
            return loggerFactory.CreateLogger<T>();
        }
    }
}
