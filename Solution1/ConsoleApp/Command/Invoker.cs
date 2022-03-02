using ConsoleApp.Command.Abstract;
using System.Threading.Tasks;

namespace ConsoleApp.Command
{
    public class Invoker
    {
        private ICommand _command;

        public void SetCommand(ICommand command)
        {
            _command = command;
        }

        public async Task RunAsync()
        {
            if (_command != null)
            {
                await _command.ExecuteAsync();
            }
        }
    }
}
