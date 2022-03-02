using System.Threading.Tasks;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Services.Abstract;

namespace ConsoleApp.Command
{
    public class ForecastWeatherCommand : ICommand
    {
        private readonly IUserCommunicateService _userCommunicateService;

        public ForecastWeatherCommand(IUserCommunicateService userCommunicateService)
        {
            _userCommunicateService = userCommunicateService;
        }

        public async Task ExecuteAsync()
        {
            await _userCommunicateService.GetForecastByCityNameAsync();
        }
    }
}
