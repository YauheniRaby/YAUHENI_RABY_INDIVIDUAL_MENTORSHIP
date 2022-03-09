using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Services.Abstract
{
    public interface IPerformerCommandsService
    {
        Task GetCurrentWeatherAsync();

        Task GetForecastByCityNameAsync();

        Task CloseApplication();
    }
}
