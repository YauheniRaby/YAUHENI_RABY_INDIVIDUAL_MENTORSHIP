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
        private readonly CancellationToken _cancellationToken;


        public CurrentWeatherCommand(IWeatherServiсe weatherServiсe, string cityName, CancellationToken cancellationToken = new CancellationToken())
        {
            _weatherServiсe = weatherServiсe;
            _cityName = cityName;
            _cancellationToken = cancellationToken;
        }

        public Task<WeatherDTO> ExecuteAsync()
        {
            return _weatherServiсe.GetByCityNameAsync(_cityName, _cancellationToken);
        }
    }
}
