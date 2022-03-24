using BusinessLayer.DTOs;
using BusinessLayer.DTOs.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IWeatherServiсe
    {
        Task<WeatherDTO> GetByCityNameAsync(string cityName);

        Task<ForecastWeatherDTO> GetForecastByCityNameAsync(string cityName, int countDay);

        Task<Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>> GetWeatherByArrayCityNameAsync(IEnumerable<string> cityNames);
    }
}
