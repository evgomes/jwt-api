namespace JWTAPI.Core.Security.Hashing;
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool ValidatePassword(string providedPassword, string passwordHash);
}