namespace WeatherApi.Configuration
{
    public interface IConfig
    {
        int MinCountDaysForecast { get; }

        int MaxCountDaysForecast { get; }

        bool IsDebugMode { get; }

        int? RequestTimeout { get; }
    }
}
