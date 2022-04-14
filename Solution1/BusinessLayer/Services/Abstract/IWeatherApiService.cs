using BusinessLayer.DTOs.WeatherAPI;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IWeatherApiService
    {
        Task<WeatherApiDTO> GetByCityNameAsync(string cityName, string currentWeatherUrl, string apiKey, CancellationToken cancellationToken);

        Task<ForecastWeatherApiDTO> GetForecastByCityNameAsync(string cityName, int countDay, string forecastWeatherUrl, string coordinatesUrl, string apiKey, CancellationToken cancellationToken);
    }
}
