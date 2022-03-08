using System.Threading.Tasks;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Services.Abstract;

namespace ConsoleApp.Command
{
    public class CurrentWeatherCommand : ICommand
    {
        private readonly IUserCommunicateService _userCommunicateService;

        public CurrentWeatherCommand(IUserCommunicateService userCommunicateService)
        {
            _userCommunicateService = userCommunicateService;
        }

        public Task<bool> ExecuteAsync()
        {
            return _userCommunicateService.GetCurrentWeatherAsync();
        }
    }
}
