using System.Threading.Tasks;

namespace BusinessLayer.Command.Abstract
{
    public interface IInvoker
    {
        void SetCommand(ICommand command);

        Task<T> RunAsync<T>();
    }
}
