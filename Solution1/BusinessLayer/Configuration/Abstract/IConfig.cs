namespace BusinessLayer.Configuration.Abstract
{
    public interface IConfig
    {
        int MinCountDaysForecast { get; }

        int MaxCountDaysForecast { get; }

        bool IsDebugMode { get; }

        int? RequestTimeout {get;}

    }
}
