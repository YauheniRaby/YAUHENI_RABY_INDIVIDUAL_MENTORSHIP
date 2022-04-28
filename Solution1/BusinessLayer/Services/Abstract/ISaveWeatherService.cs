using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface ISaveWeatherService
    {
        Task AddByArrayCityNameAsync(IEnumerable<string> cities, string currentWeatherUrl, CancellationToken token);
    }
}
