using System.Threading.Tasks;
using JWTAPI.Core.Models;

namespace JWTAPI.Core.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user, ERole[] userRoles);
        Task<User> FindAsync(string email);
    }
}