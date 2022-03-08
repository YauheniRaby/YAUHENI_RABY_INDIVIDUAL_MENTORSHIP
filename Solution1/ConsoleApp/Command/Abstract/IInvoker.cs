using System.Threading.Tasks;

namespace ConsoleApp.Command.Abstract
{
    public interface IInvoker
    {
        void SetCommand(ICommand command);

        Task<bool> RunAsync();
    }
}
