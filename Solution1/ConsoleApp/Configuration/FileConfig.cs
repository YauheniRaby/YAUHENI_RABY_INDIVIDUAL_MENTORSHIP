﻿using System.Configuration;
using BusinessLayer.Configuration.Abstract;

namespace ConsoleApp.Configuration
{
    public class FileConfig : IConfig
    {
        public int MinCountDaysForecast => int.TryParse(ConfigurationManager.AppSettings["minCountDays"], out var value) ? value : default;

        public int MaxCountDaysForecast => int.TryParse(ConfigurationManager.AppSettings["maxCountDays"], out var value) ? value : default;

        public bool IsDebugMode => bool.TryParse(ConfigurationManager.AppSettings["isDebugMode"], out var value) ? value : default;
    }
}