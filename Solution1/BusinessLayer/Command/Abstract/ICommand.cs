using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Command.Abstract
{
    public interface ICommand<T>
    {
        Task<T> ExecuteAsync(CancellationToken cancelationToken);
    }
}
