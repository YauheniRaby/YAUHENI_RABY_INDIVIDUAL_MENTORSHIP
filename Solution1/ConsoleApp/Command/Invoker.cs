using System.Threading.Tasks;
using ConsoleApp.Command.Abstract;

namespace ConsoleApp.Command
{
    public class Invoker : IInvoker
    {
        private ICommand _command;

        public void SetCommand(ICommand command)
        {
            _command = command;
        }

        public Task<bool> RunAsync()
        {
            return _command?.ExecuteAsync();
        }
    }
}
