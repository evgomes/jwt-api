using JWTAPI.Core.Security.Hashing;
using JWTAPI.Persistence;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace JWTAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<AppDbContext>();
                var passwordHasher = services.GetService<IPasswordHasher>();
                DatabaseSeed.Seed(context, passwordHasher);
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            /*
             * The call to ".UseIISIntegration" is necessary to fix issue while running the API from ISS. See the following links for reference:
             * - https://github.com/aspnet/IISIntegration/issues/242
             * - https://stackoverflow.com/questions/50112665/newly-created-net-core-gives-http-400-using-windows-authentication
            */

            WebHost.CreateDefaultBuilder(args)
                   .UseIISIntegration()
                   .UseStartup<Startup>()
                   .Build();
    }
}