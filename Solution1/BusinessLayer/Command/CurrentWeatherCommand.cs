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

        public CurrentWeatherCommand(IWeatherServiсe weatherServiсe, string cityName)
        {
            _weatherServiсe = weatherServiсe;
            _cityName = cityName;
        }

        public Task<WeatherDTO> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _weatherServiсe.GetByCityNameAsync(_cityName, cancellationToken);
        }
    }
}
