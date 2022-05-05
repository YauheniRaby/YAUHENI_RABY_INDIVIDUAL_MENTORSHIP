using DataAccessLayer.Models;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Abstract
{
    public interface IUserRepository
    {
        User GetByEmail(string email);

        User GetByIdAsync(int id);
    }
}
