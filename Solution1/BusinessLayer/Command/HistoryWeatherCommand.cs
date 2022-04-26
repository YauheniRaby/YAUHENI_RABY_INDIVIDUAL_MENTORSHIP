using BusinessLayer.Command.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Command
{
    public  class HistoryWeatherCommand : ICommand<HistoryWeatherDTO>
    {
        readonly IHistoryWeatherService _historyWeatherService;
        readonly HistoryWeatherRequestDTO _requestHistoryWeatherDTO;

        public HistoryWeatherCommand(IHistoryWeatherService historyWeatherService, HistoryWeatherRequestDTO requestHistoryWeatherDTO)
        {
            _historyWeatherService = historyWeatherService;
            _requestHistoryWeatherDTO = requestHistoryWeatherDTO;
        }

        public Task<HistoryWeatherDTO> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _historyWeatherService.GetByCityNameAndPeriodAsync(_requestHistoryWeatherDTO, cancellationToken);
        }
    }
}
