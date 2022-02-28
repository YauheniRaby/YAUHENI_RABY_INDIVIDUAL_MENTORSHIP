using System.Threading.Tasks;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Services.Abstract;

namespace ConsoleApp.Command
{
    public class CurrentlyWeatherCommand : ICommand
    {
        private readonly IUserCommunicateService _userCommunicateService;

        public CurrentlyWeatherCommand(IUserCommunicateService userCommunicateService)
        {
            _userCommunicateService = userCommunicateService;
        }

        public async Task Execute()
        {
            await _userCommunicateService.GetCurrentlyWeatherAsync();
        }
    }
}
