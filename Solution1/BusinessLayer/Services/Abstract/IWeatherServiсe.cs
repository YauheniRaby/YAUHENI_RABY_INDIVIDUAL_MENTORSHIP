using BusinessLayer.DTOs;
using BusinessLayer.DTOs.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IWeatherServiсe
    {
        Task<WeatherDTO> GetByCityNameAsync(string cityName, string currentWeatherUrl, CancellationToken cancellationToken);

        Task<ForecastWeatherDTO> GetForecastByCityNameAsync(string cityName, int countDay, string forecastWeatherUrl, string coordinatesUrl, int countWeatherPointInDay, CancellationToken cancellationToken);

        Task<Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>> GetWeatherByArrayCityNameAsync(IEnumerable<string> cityNames, string currentWeatherUrl, CancellationToken cancellationToken);       
    }
}
