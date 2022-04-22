using System.Configuration;

namespace ConsoleApp.Configuration
{
    public class AppConfiguration
    {
        public AppConfiguration()
        {
            MinCountDaysForecast = int.TryParse(ConfigurationManager.AppSettings[nameof(MinCountDaysForecast)], out int minCountDays) ? minCountDays : default;

            MaxCountDaysForecast = int.TryParse(ConfigurationManager.AppSettings[nameof(MaxCountDaysForecast)], out int maxCountDays) ? maxCountDays : default;

            IsDebugMode = bool.TryParse(ConfigurationManager.AppSettings[nameof(IsDebugMode)], out bool isDebug) ? isDebug : default;

            RequestTimeout = int.TryParse(ConfigurationManager.AppSettings[nameof(RequestTimeout)], out int timeout) ? timeout : default;
        }

        public int MinCountDaysForecast { get; set; }

        public int MaxCountDaysForecast { get; set; }

        public bool IsDebugMode { get; set; }

        public int? RequestTimeout { get; set; }
    }
}
