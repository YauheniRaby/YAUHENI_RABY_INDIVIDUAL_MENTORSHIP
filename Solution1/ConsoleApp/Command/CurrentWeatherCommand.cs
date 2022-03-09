using System.Threading.Tasks;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Services.Abstract;

namespace ConsoleApp.Command
{
    public class CurrentWeatherCommand : ICommand
    {
        private readonly IPerformerCommandsService _performerCommandsService;

        public CurrentWeatherCommand(IPerformerCommandsService performerCommandsService)
        {
            _performerCommandsService = performerCommandsService;
        }

        public Task ExecuteAsync()
        {
            return _performerCommandsService.GetCurrentWeatherAsync();
        }
    }
}
