namespace ConsoleApp.Configuration.Abstract
{
    public interface IConfig
    {
        AppConfiguration AppConfig { get; }

        WeatherAriConfiguration ApiConfig { get; }

        DbConfiguration DbConfig { get; }
    }
}
