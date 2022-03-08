using System;
using System.Threading.Tasks;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Services.Abstract;

namespace ConsoleApp.Command
{
    public class ExitCommand : ICommand
    {
        private readonly ICloseApplicationService _closeApplicationService;

        public ExitCommand(ICloseApplicationService closeApplicationService)
        {
            _closeApplicationService = closeApplicationService;
        }

        public Task<bool> ExecuteAsync()
        {
            return _closeApplicationService.Exit();
        }
    }
}
