using BusinessLayer.DTOs;
using BusinessLayer.DTOs.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IWeatherServiсe
    {
        Task<WeatherDTO> GetByCityNameAsync(string cityName, string currentWeatherUrl, string apiKey, CancellationToken cancellationToken);

        Task<ForecastWeatherDTO> GetForecastByCityNameAsync(string cityName, int countDay, string forecastWeatherUrl, string coordinatesUrl, string apiKey, int countWeatherPointInDay, CancellationToken cancellationToken);

        Task<Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>> GetWeatherByArrayCityNameAsync(IEnumerable<string> cityNames, string currentWeatherUrl, string apiKey, CancellationToken cancellationToken);

        Task SaveWeatherListAsync(IEnumerable<string> cities, string currentWeatherUrl, string apiKey);
    }
}
