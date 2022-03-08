using System.Threading.Tasks;

namespace ConsoleApp.Services.Abstract
{
    public interface IUserCommunicateService
    {
        Task<bool> GetCurrentWeatherAsync();

        Task<bool> GetForecastByCityNameAsync();

        Task<bool> CommunicateAsync();
    }
}
