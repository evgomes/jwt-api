namespace JWTAPI.Core.Security.Tokens;
public class RefreshTokenWithEmail
{
	public string Email { get; set; } = null!;
	public RefreshToken RefreshToken { get; set; } = null!;
}
