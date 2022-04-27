using BusinessLayer.DTOs;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IHistoryWeatherService
    {
        Task<HistoryWeatherDTO> GetByCityNameAndPeriodAsync(HistoryWeatherRequestDTO historyWeatherRequestDTO, CancellationToken token);

        Task BulkSaveWeatherListAsync(IEnumerable<Weather> weatherList, CancellationToken token);
    }
}
