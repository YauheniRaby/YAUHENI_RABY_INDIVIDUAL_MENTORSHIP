using BusinessLayer.DTOs;
using System.Threading.Tasks;

namespace BusinessLayer.Service.Abstract
{
    public interface IWeatherApiService
    {
        Task<WeatherApiDTO> GetByCityNameAsync(string cityName);
    }
}
