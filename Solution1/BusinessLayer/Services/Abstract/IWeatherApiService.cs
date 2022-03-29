using BusinessLayer.DTOs.WeatherAPI;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IWeatherApiService
    {
        Task<WeatherApiDTO> GetByCityNameAsync(string cityName, CancellationToken cancellationToken);

        Task<ForecastWeatherApiDTO> GetForecastByCityNameAsync(string cityName, int countDay, CancellationToken cancellationToken);
    }
}
