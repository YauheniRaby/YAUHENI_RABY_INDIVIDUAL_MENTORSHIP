using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Command.Abstract
{
    public interface IInvoker
    {
        Task<T> RunAsync<T>(ICommand<T> command, CancellationToken cancelationToken);
    }
}
