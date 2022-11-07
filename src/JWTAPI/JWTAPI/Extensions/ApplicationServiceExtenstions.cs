namespace JWTAPI.Extensions;
public static class ApplicationServiceExtenstions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddControllers();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("jwtapi");
        });

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddSingleton<ITokenHandler, Security.Tokens.TokenHandler>();

        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
