using BusinessLayer.DTOs.WeatherAPI;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IWeatherApiService
    {
        Task<WeatherApiDTO> GetByCityNameAsync(string cityName, string currentWeatherUrl, CancellationToken cancellationToken);

        Task<ForecastWeatherApiDTO> GetForecastByCityNameAsync(string cityName, int countDay, string forecastWeatherUrl, string coordinatesUrl, CancellationToken cancellationToken);
    }
}
