namespace JWTAPI.Controllers.Resources;

public class AccessTokenResource
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
	public long Expiration { get; set; }
}