namespace JWTAPI.Security.Tokens;

public class TokenHandler : ITokenHandler
{
    private readonly ISet<RefreshTokenWithEmail> _refreshTokens = new HashSet<RefreshTokenWithEmail>();

    private readonly TokenOptions _tokenOptions;
    private readonly SigningConfigurations _signingConfigurations;
    private readonly IPasswordHasher _passwordHaser;

    public TokenHandler(
        IOptions<TokenOptions> tokenOptionsSnapshot,
        SigningConfigurations signingConfigurations,
        IPasswordHasher passwordHaser)
    {
        _passwordHaser = passwordHaser;
        _tokenOptions = tokenOptionsSnapshot.Value;
        _signingConfigurations = signingConfigurations;
    }

    public AccessToken CreateAccessToken(User user)
    {
        var refreshToken = BuildRefreshToken();
        var accessToken = BuildAccessToken(user, refreshToken);

        _refreshTokens.Add(new RefreshTokenWithEmail
        {
            Email = user.Email,
            RefreshToken = refreshToken,
        });

        return accessToken;
    }

    public RefreshToken? TakeRefreshToken(string token, string userEmail)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(userEmail))
        {
            return null;
        }

        var refreshTokenWithEmail = _refreshTokens.SingleOrDefault(t => t.RefreshToken.Token == token && t.Email == userEmail);

        if (refreshTokenWithEmail == null)
        {
            return null;
        }

        _refreshTokens.Remove(refreshTokenWithEmail);

        return refreshTokenWithEmail.RefreshToken;
    }

    public void RevokeRefreshToken(string token, string userEmail)
    {
        TakeRefreshToken(token, userEmail);
    }

    private RefreshToken BuildRefreshToken()
    {
        var refreshToken = new RefreshToken
        (
            token: _passwordHaser.HashPassword(Guid.NewGuid().ToString()),
            expiration: DateTime.UtcNow.AddSeconds(_tokenOptions.RefreshTokenExpiration).Ticks
        );

        return refreshToken;
    }

    private AccessToken BuildAccessToken(User user, RefreshToken refreshToken)
    {
        var accessTokenExpiration = DateTime.UtcNow.AddSeconds(_tokenOptions.AccessTokenExpiration);

        var securityToken = new JwtSecurityToken
        (
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: GetClaims(user),
            expires: accessTokenExpiration,
            notBefore: DateTime.UtcNow,
            signingCredentials: _signingConfigurations.SigningCredentials
        );

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(securityToken);

        return new AccessToken(accessToken, accessTokenExpiration.Ticks, refreshToken);
    }

    private IEnumerable<Claim> GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email)
        };

        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
        }

        return claims;
    }
}
