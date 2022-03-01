using BusinessLayer.DTOs;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IWeatherApiService
    {
        Task<WeatherApiDTO> GetByCityNameAsync(string cityName);

        Task<ForecastWeatherApiDTO> GetForecastByCityNameAsync(string cityName, int countDay);
    }
}
