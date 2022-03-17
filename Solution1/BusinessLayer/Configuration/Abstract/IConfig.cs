using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Configuration.Abstract
{
    public interface IConfig
    {
        int MinCountDaysForecast { get; }

        int MaxCountDaysForecast { get; }

        bool IsDebugMode { get; }
    }
}
