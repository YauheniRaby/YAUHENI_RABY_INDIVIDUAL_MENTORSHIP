using BusinessLayer.DTOs;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IWeatherServiсe
    {
        Task<WeatherDTO> GetByCityNameAsync(string cityName);
    }
}
