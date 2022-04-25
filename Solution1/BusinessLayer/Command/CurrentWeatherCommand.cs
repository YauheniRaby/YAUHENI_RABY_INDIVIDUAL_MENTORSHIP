using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Command.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;

namespace BusinessLayer.Command
{
    public class CurrentWeatherCommand : ICommand<WeatherDTO>
    {
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly string _cityName;
        private readonly string _currentWeatherUrl;

        public CurrentWeatherCommand(IWeatherServiсe weatherServiсe, string cityName, string currentWeatherUrl)
        {
            _weatherServiсe = weatherServiсe;
            _cityName = cityName;
            _currentWeatherUrl = currentWeatherUrl;  
        }

        public Task<WeatherDTO> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _weatherServiсe.GetByCityNameAsync(_cityName, _currentWeatherUrl, cancellationToken);
        }
    }
}
