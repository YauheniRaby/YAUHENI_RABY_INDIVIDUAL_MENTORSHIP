using System.Threading.Tasks;
using BusinessLayer.Command.Abstract;

namespace BusinessLayer.Command
{
    public class Invoker : IInvoker
    {
        private ICommand _command;

        public void SetCommand(ICommand command)
        {
            _command = command;
        }

        public async Task<T> RunAsync<T>()
        {
            return (T) await _command?.ExecuteAsync();
        }
    }
}
