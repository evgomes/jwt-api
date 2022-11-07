namespace JWTAPI.Core.Repositories;
public interface IUserRepository
{
    Task AddAsync(User user, ApplicationRole[] userRoles);
    Task<User> FindByEmailAsync(string email);
}