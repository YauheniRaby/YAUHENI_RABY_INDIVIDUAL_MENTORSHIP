using System.Threading.Tasks;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Services.Abstract;

namespace ConsoleApp.Command
{
    public class ForecastWeatherCommand : ICommand
    {
        private readonly IPerformerCommandsService _performerCommandsService;

        public ForecastWeatherCommand(IPerformerCommandsService performerCommandsService)
        {
            _performerCommandsService = performerCommandsService;
        }

        public Task ExecuteAsync()
        {
            return _performerCommandsService.GetForecastByCityNameAsync();
        }
    }
}
