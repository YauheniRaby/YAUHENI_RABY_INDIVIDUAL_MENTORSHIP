using BusinessLayer.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IHistoryWeatherService
    {
        Task<HistoryWeatherDTO> GetByCityNameAndPeriodAsync(HistoryWeatherRequestDTO historyWeatherRequestDTO, CancellationToken token);
    }
}
