namespace WeatherApi.Configuration
{
    public interface IAppParams
    {
        int MinCountDaysForecast { get; }

        int MaxCountDaysForecast { get; }

        bool IsDebugMode { get; }

        int? RequestTimeout { get; }
    }
}
