using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Abstract
{
    public interface IWeatherRepository
    {
        Task BulkSaveWeatherListAsync(IEnumerable<Weather> weatherList, CancellationToken token);

        Task<IEnumerable<Weather>> GetWeatherListAsync(HistoryWeatherRequest historyWeatherRequest, CancellationToken token);
    }
}
