namespace JWTAPI.Security.Hashing;

/// <summary>
/// This password hasher is the same used by ASP.NET Identity.
/// Explanation: https://stackoverflow.com/questions/20621950/asp-net-identity-default-password-hasher-how-does-it-work-and-is-it-secure
/// Full implementation: https://gist.github.com/malkafly/e873228cb9515010bdbe
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        byte[] salt;
        byte[] passwordHashKey;
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException(nameof(password));
        }
        using (Rfc2898DeriveBytes Key = new Rfc2898DeriveBytes(password, 0x10, 0x3e8, HashAlgorithmName.SHA512))
        {
            salt = Key.Salt;
            passwordHashKey = Key.GetBytes(0x20);
        }
        byte[] storingPasswordArray = new byte[0x31];
        Buffer.BlockCopy(salt, 0, storingPasswordArray, 1, 0x10);
        Buffer.BlockCopy(passwordHashKey, 0, storingPasswordArray, 0x11, 0x20);
        return Convert.ToBase64String(storingPasswordArray);
    }

    public bool ValidatePassword(string userInputData, string storedPasswordHash)
    {
        if (string.IsNullOrEmpty(storedPasswordHash))
            return false;
        if (userInputData is null)
            throw new ArgumentNullException(userInputData, "User Password should not be null !");

        byte[] decodedStoredPasswordHash = Convert.FromBase64String(storedPasswordHash);

        if (IsInvalid(decodedStoredPasswordHash))
            return false;

        byte[] salt = ExtractPortion(decodedStoredPasswordHash, 1, 0x10);
        byte[] expectedPassword = ExtractPortion(decodedStoredPasswordHash,0x11, 0x20);

        byte[] GeneratedKey = DeriveKeyFromPassword(userInputData,salt);

        return ByteArraysEqual(expectedPassword, GeneratedKey);
       
    }

    private bool IsInvalid(byte[] hashed)
    {
        return hashed.Length != 0x31 || hashed[0] != 0;
    }

    private byte[] DeriveKeyFromPassword(string userInputData, byte[] salt)
    {
        using (Rfc2898DeriveBytes key = new(userInputData,salt, 0x3e8, HashAlgorithmName.SHA512))
        {
            return key.GetBytes(0x20);
        }
    }

    private byte[] ExtractPortion(byte[] source, int offset, int length)
    {
        byte[] result = new byte[length];
        Buffer.BlockCopy(source, offset, result, 0, length);
        return result;
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    private bool ByteArraysEqual(byte[] a, byte[] b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (a == null || b == null || a.Length != b.Length)
            return false;

        return a.SequenceEqual(b);
    }
}