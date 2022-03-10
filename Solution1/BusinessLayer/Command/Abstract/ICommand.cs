using System.Threading.Tasks;

namespace BusinessLayer.Command.Abstract
{
    public interface ICommand
    {
        Task<object> ExecuteAsync();
    }
}
