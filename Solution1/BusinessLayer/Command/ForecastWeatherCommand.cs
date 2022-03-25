using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Command.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;

namespace BusinessLayer.Command
{
    public class ForecastWeatherCommand: ICommand<ForecastWeatherDTO>
    {
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly string _cityName;
        private readonly int _countDay;
        private readonly CancellationToken _cancellationToken;

        public ForecastWeatherCommand(IWeatherServiсe weatherServiсe, string cityName, int countDay, CancellationToken cancellationToken = new CancellationToken())
        {
            _weatherServiсe = weatherServiсe;
            _cityName = cityName;
            _countDay = countDay;
            _cancellationToken = cancellationToken;
        }

        public Task<ForecastWeatherDTO> ExecuteAsync()
        {
            return _weatherServiсe.GetForecastByCityNameAsync(_cityName, _countDay, _cancellationToken);
        }
    }
}
