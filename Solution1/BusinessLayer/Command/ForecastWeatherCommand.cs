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

        public ForecastWeatherCommand(IWeatherServiсe weatherServiсe, string cityName, int countDay)
        {
            _weatherServiсe = weatherServiсe;
            _cityName = cityName;
            _countDay = countDay;
        }

        public Task<ForecastWeatherDTO> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _weatherServiсe.GetForecastByCityNameAsync(_cityName, _countDay, cancellationToken);
        }
    }
}
