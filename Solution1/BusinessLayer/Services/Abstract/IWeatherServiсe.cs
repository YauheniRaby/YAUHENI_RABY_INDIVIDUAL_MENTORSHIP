using BusinessLayer.DTOs;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IWeatherServiсe
    {
        Task<WeatherDTO> GetByCityNameAsync(string cityName);

        Task<ForecastWeatherDTO> GetForecastByCityNameAsync(string cityName, int countDay);
    }
}
