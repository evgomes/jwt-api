namespace JWTAPI.Models.Security
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool PasswordMatches(string providedPassword, string hashedPassword);
    }
}