using ConsoleApp.Configuration.Abstract;

namespace ConsoleApp.Configuration
{
    public class FileConfig : IConfig
    {
        public AppConfiguration AppConfiguration { get; set; }

        public WeatherApiConfiguration WeatherApiConfiguration { get; set; }
    }
}
