namespace JWTAPI.Persistence;

/// <summary>
/// EF Core already supports database seeding throught overriding "OnModelCreating", but I decided to create a separate seed class to avoid 
/// injecting IPasswordHasher into AppDbContext.
/// To understand how to use database seeding into DbContext classes, check this link: https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding
/// </summary>
public class DatabaseSeed
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        context.Database.EnsureCreated();

        if (await context.Roles.AnyAsync()) return;

        var roles = new List<Role>
        {
            new Role { Name = ApplicationRole.Common.ToString() },
            new Role { Name = ApplicationRole.Administrator.ToString() }
        };

        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();

        var users = new List<User>
        {
            new User { Email = "admin@admin.com", Password = passwordHasher.HashPassword("12345678") },
            new User { Email = "common@common.com", Password = passwordHasher.HashPassword("12345678") },
        };

        users[0].UserRoles.Add(new UserRole
        {
            RoleId = context.Roles.Single(r => r.Name == ApplicationRole.Administrator.ToString()).Id,
            Role = new Role
            {
                Name = ApplicationRole.Administrator.ToString()
            }
        });

        users[1].UserRoles.Add(new UserRole
        {
            RoleId = context.Roles.Single(r => r.Name == ApplicationRole.Common.ToString()).Id,
            Role = new Role
            {
                Name = ApplicationRole.Common.ToString()
            }
        });

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }
}