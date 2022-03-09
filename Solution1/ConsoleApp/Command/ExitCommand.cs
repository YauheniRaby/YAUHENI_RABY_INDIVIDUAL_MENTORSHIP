using System;
using System.Threading.Tasks;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Services.Abstract;

namespace ConsoleApp.Command
{
    public class ExitCommand : ICommand
    {
        private readonly IPerformerCommandsService _performerCommandsService;

        public ExitCommand(IPerformerCommandsService performerCommandsService)
        {
            _performerCommandsService = performerCommandsService;
        }

        public Task ExecuteAsync()
        {
            return _performerCommandsService.CloseApplication();
        }
    }
}
