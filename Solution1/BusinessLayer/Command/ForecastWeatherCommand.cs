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
        private readonly string _forecastWeatherUrl;
        private readonly string _coordinatesUrl;
        private readonly string _apiKey;
        private readonly int _countWeatherPointInDay;

        public ForecastWeatherCommand(IWeatherServiсe weatherServiсe, string cityName, int countDay, string forecastWeatherUrl, string coordinatesUrl, string apiKey, int countWeatherPointInDay)
        {
            _weatherServiсe = weatherServiсe;
            _cityName = cityName;
            _countDay = countDay;
            _forecastWeatherUrl = forecastWeatherUrl;
            _coordinatesUrl = coordinatesUrl;
            _apiKey = apiKey;
            _countWeatherPointInDay = countWeatherPointInDay;
        }

        public Task<ForecastWeatherDTO> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _weatherServiсe.GetForecastByCityNameAsync(_cityName, _countDay, _forecastWeatherUrl, _coordinatesUrl, _apiKey, _countWeatherPointInDay, cancellationToken);
        }
    }
}
