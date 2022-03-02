using System.Threading.Tasks;

namespace ConsoleApp.Services.Abstract
{
    public interface IUserCommunicateService
    {
        Task GetCurrentlyWeatherAsync();

        Task GetForecastByCityNameAsync();

        Task StartUpApplicationAsync();
    }
}
