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
        private readonly string _currentWeatherUrl;

        public BestWeatherCommand(IWeatherServiсe weatherServiсe, IEnumerable<string> cityNames, string currentWeatherUrl)
        {
            _weatherServiсe = weatherServiсe;
            _cityNames = cityNames;     
            _currentWeatherUrl = currentWeatherUrl;
        }

        public Task<Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>> ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _weatherServiсe.GetWeatherByArrayCityNameAsync(_cityNames, _currentWeatherUrl, cancellationToken);
        }
    }
}
