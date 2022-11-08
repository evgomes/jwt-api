var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomSwagger();

builder.Services.AddApplicationServices();

builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseRouting();

app.UseCustomSwagger();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

using var scope = app.Services.CreateScope();
try
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DatabaseSeed.SeedAsync(dbContext, passwordHasher);
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured while applying migrations");
}

await app.RunAsync();