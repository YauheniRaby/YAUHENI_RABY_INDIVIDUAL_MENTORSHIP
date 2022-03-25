using BusinessLayer.Command.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.DTOs.Enums;
using BusinessLayer.Services.Abstract;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Command
{
    public class BestWeatherCommand : ICommand<Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>>
    {
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly IEnumerable<string> _cityNames;
        private readonly CancellationToken _cancellationToken;

        public BestWeatherCommand(IWeatherServiсe weatherServiсe, IEnumerable<string> cityNames, CancellationToken cancellationToken = new CancellationToken())
        {
            _weatherServiсe = weatherServiсe;
            _cityNames = cityNames;
            _cancellationToken = cancellationToken;
        }

        public Task<Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>> ExecuteAsync()
        {
            return _weatherServiсe.GetWeatherByArrayCityNameAsync(_cityNames, _cancellationToken);
        }
    }
}
