using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Command.Abstract;

namespace BusinessLayer.Command
{
    public class Invoker : IInvoker
    {
        public Task<T> RunAsync<T>(ICommand<T> command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return command.ExecuteAsync(cancellationToken);
        }
    }
}
