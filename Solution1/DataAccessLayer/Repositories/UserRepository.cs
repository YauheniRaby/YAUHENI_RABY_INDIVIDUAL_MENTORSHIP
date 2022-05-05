using DataAccessLayer.Configuration;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Abstract;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RepositoryContext _context;

        public UserRepository(RepositoryContext context)
        {
            _context = context;
        }

        public User GetByEmail(string email)
        {
            return _context.Users.SingleOrDefault(x => x.Email == email);
        }

        public User GetByIdAsync(int id)
        {
            return _context.Users.SingleOrDefault(x => x.Id == id);
        }
    }
}
