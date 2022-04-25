namespace ConsoleApp.Configuration.Abstract
{
    public interface IConfig
    {
        AppConfiguration AppConfiguration { get; }

        WeatherApiConfiguration WeatherApiConfiguration { get; }
    }
}
