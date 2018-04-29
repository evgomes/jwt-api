using System.Threading.Tasks;

namespace JWTAPI.Models.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user, ERole[] userRoles);
        Task<User> FindAsync(string email);
    }
}