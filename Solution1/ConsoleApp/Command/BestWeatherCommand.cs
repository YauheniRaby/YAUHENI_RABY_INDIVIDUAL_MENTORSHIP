using System.Threading.Tasks;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Services.Abstract;

namespace ConsoleApp.Command
{
    internal class BestWeatherCommand : ICommand
    {
        private readonly IPerformerCommandsService _performerCommandsService;

        public BestWeatherCommand(IPerformerCommandsService performerCommandsService)
        {
            _performerCommandsService = performerCommandsService;
        }

        public Task ExecuteAsync()
        {
            return _performerCommandsService.GetBestWeatherAsync();
        }
    }
}
