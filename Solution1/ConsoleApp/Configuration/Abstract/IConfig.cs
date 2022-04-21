namespace ConsoleApp.Configuration.Abstract
{
    public interface IConfig
    {
        AppConfiguration AppConfig { get; }

        WeatherApiConfiguration ApiConfig { get; }
    }
}
