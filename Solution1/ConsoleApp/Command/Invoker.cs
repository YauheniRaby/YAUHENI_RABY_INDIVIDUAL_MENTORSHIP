using System.Threading.Tasks;
using ConsoleApp.Command.Abstract;

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
            await _command?.ExecuteAsync();
        }
    }
}
