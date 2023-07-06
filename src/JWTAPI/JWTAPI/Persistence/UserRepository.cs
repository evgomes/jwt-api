namespace JWTAPI.Persistence;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user, ApplicationRole[] userRoles)
    {
        var roleNames = userRoles.Select(r => r.ToString()).ToList();
        var roles = await _context.Roles.Where(r => roleNames.Contains(r.Name)).ToListAsync();

        foreach (var role in roles)
        {
            user.UserRoles.Add(new UserRole { RoleId = role.Id });
        }

        _context.Users.Add(user);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _context.Users
            .Include(_ => _.UserRoles)
                .ThenInclude(_ => _.Role)
            .FirstOrDefaultAsync(_ => _.Email.Equals(email));
    }
}