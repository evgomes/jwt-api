namespace JWTAPI.Core.Security.Tokens;
public class RefreshTokenWithEmail
{
	public string Email { get; set; }
	public RefreshToken RefreshToken { get; set; }
}
