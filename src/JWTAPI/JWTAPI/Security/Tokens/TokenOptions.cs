namespace JWTAPI.Security.Tokens;
public class TokenOptions
{
	public string Audience { get; set; } = null!;
	public string Issuer { get; set; } = null!;
	public long AccessTokenExpiration { get; set; }
	public long RefreshTokenExpiration { get; set; }
	public string Secret { get; set; } = null!;
}