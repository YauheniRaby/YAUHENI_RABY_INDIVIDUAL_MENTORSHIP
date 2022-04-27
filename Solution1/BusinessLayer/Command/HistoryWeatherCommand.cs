using BusinessLayer.Command.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Command
{
    public  class HistoryWeatherCommand : ICommand<IEnumerable<WeatherWithDateTimeDTO>>
    {
        private readonly IHistoryWeatherService _historyWeatherService;
        private readonly HistoryWeatherRequestDTO _requestHistoryWeatherDTO;

        public HistoryWeatherCommand(IHistoryWeatherService historyWeatherService, HistoryWeatherRequestDTO requestHistoryWeatherDTO)
        {
            _historyWeatherService = historyWeatherService;
            _requestHistoryWeatherDTO = requestHistoryWeatherDTO;
        }

        public Task<IEnumerable<WeatherWithDateTimeDTO>> ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _historyWeatherService.GetByCityNameAndPeriodAsync(_requestHistoryWeatherDTO, cancellationToken);
        }
    }
}
