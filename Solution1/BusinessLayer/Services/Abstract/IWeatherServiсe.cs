using BusinessLayer.DTOs;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IWeatherServiсe
    {
        Task<WeatherDTO> GetByCityNameAsync(ForecastWeatherRequestDTO dataForWeatherRequestDTO);

        Task<ForecastWeatherDTO> GetForecastByCityNameAsync(ForecastWeatherRequestDTO dataForWeatherRequestDTO);
    }
}
