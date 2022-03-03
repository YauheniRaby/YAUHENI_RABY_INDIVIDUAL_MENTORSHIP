using System.Threading.Tasks;

namespace ConsoleApp.Services.Abstract
{
    public interface IUserCommunicateService
    {
        Task GetCurrentWeatherAsync();

        Task GetForecastByCityNameAsync();

        Task CommunicateAsync();
    }
}
